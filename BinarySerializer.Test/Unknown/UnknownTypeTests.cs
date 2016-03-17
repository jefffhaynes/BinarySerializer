using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Unknown
{
    [TestClass]
    public class UnknownTypeTests : TestBase
    {
        [TestMethod]
        public void UnknownTypeSerializationTest()
        {
            var unknownTypeClass = new UnknownTypeClass {Field = "hello"};

            var serializer = new BinarySerializer();

            var stream = new MemoryStream();
            serializer.Serialize(stream, unknownTypeClass);
        }

        [TestMethod]
        [ExpectedException(typeof (BindingException))]
        public void SubtypesOnUnknownTypeFieldShouldThrowBindingException()
        {
            var unknownTypeClass = new InvalidUnknownTypeClass { Field = "hello" };

            var serializer = new BinarySerializer();

            var stream = new MemoryStream();
            serializer.Serialize(stream, unknownTypeClass);
        }

        [TestMethod]
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

            Assert.AreEqual((byte)childClass.Subfield.Length, data[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ObjectSerializationShouldThrow()
        {
            var unknownTypeClass = new UnknownTypeClass { Field = new object() };
            Roundtrip(unknownTypeClass);
        }
    }
}
