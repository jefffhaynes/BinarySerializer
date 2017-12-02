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
        public void SubtypesOnUnknownTypeFieldShouldThrowBindingException()
        {
            var unknownTypeClass = new InvalidUnknownTypeClass {Field = "hello"};

            var serializer = new BinarySerializer();

            var stream = new MemoryStream();
            Assert.Throws<BindingException>(() => serializer.Serialize(stream, unknownTypeClass));
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

            Assert.Equal((byte) childClass.Subfield.Length, data[0]);
        }

        [Fact]
        public void ObjectSerializationShouldThrow()
        {
            var unknownTypeClass = new UnknownTypeClass {Field = new object()};
#if TESTASYNC
            Assert.Throws<AggregateException>(() => Roundtrip(unknownTypeClass));
#else
            Assert.Throws<InvalidOperationException>(() => Roundtrip(unknownTypeClass));
#endif
        }
    }
}