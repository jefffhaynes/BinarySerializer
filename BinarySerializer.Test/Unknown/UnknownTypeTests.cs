using System.IO;
using BinarySerialization;
using BinarySerialization.Test.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Unknown
{
    [TestClass]
    public class UnknownTypeTests
    {
        [TestMethod]
        public void UnknownTypeSerializationTest()
        {
            var unknownTypeClass = new UnknownTypeClass {Field = "hello"};

            var serializer = new BinarySerialization.BinarySerializer();

            var stream = new MemoryStream();
            serializer.Serialize(stream, unknownTypeClass);
        }

        [TestMethod]
        [ExpectedException(typeof (BindingException))]
        public void SubtypesOnUnknownTypeFieldShouldThrowBindingException()
        {
            var unknownTypeClass = new InvalidUnknownTypeClass { Field = "hello" };

            var serializer = new BinarySerialization.BinarySerializer();

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

            var serializer = new BinarySerialization.BinarySerializer();

            var stream = new MemoryStream();
            serializer.Serialize(stream, unknownTypeClass);

            var data = stream.ToArray();

            Assert.AreEqual((byte)childClass.Subfield.Length, data[0]);
        }
    }
}
