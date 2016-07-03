using System;
using System.IO;
using Xunit;

namespace BinarySerialization.Test.Unknown
{
        public class UnknownTypeTests : TestBase
    {
        [Fact]
        public void UnknownTypeSerializationTest()
        {
            var unknownTypeClass = new UnknownTypeClass {Field = "hello"};

            var serializer = new BinarySerializer();

            var stream = new MemoryStream();
            serializer.Serialize(stream, unknownTypeClass);
        }

        [Fact]
        //[ExpectedException(typeof (BindingException))]
        public void SubtypesOnUnknownTypeFieldShouldThrowBindingException()
        {
            var unknownTypeClass = new InvalidUnknownTypeClass { Field = "hello" };

            var serializer = new BinarySerializer();

            var stream = new MemoryStream();
            var e = Record.Exception(() => serializer.Serialize(stream, unknownTypeClass));
            Assert.NotNull(e);
            Assert.IsType<BindingException>(e);
        }

        [Fact]
        public void BindingAcrossUnknownBoundaryTest()
        {
            var childClass = new BindingAcrossUnknownBoundaryChildClass {Subfield = "hello"};
            var unknownTypeClass = new BindingAcrossUnknownBoundaryClass
            {
                Field = childClass
            };

            var serializer = new BinarySerializer();

            var stream = new MemoryStream();
            serializer.Serialize(stream, unknownTypeClass);

            var data = stream.ToArray();

            Assert.Equal((byte)childClass.Subfield.Length, data[0]);
        }

        [Fact]
        //[ExpectedException(typeof(InvalidOperationException))]
        public void ObjectSerializationShouldThrow()
        {
            var unknownTypeClass = new UnknownTypeClass { Field = new object() };
            var e = Record.Exception(() => Roundtrip(unknownTypeClass));
            Assert.NotNull(e);
            Assert.IsType<InvalidOperationException>(e);
        }
    }
}
