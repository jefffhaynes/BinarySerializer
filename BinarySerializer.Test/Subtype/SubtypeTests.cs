using System;
using BinarySerialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Subtype
{
    [TestClass]
    public class SubtypeTests : TestBase
    {
        [TestMethod]
        public void SubtypeTest()
        {
            var expected = new SubtypeClass {Field = new SubclassB()};
            var actual = Roundtrip(expected);

            Assert.AreEqual(SubclassType.B, actual.Subtype);
            Assert.IsInstanceOfType(actual.Field, typeof(SubclassB));
        }

        [TestMethod]
        public void SubSubtypeTest()
        {
            var expected = new SubtypeClass { Field = new SubSubclassC() };
            var actual = Roundtrip(expected);

            Assert.AreEqual(SubclassType.C, actual.Subtype);
            Assert.IsInstanceOfType(actual.Field, typeof(SubSubclassC));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MissingSubtypeTest()
        {
            var expected = new IncompleteSubtypeClass { Field = new SubclassB() };
            Roundtrip(expected);
        }

        [TestMethod]
        public void BestFitSubtypeTest()
        {
            var expected = new SubtypeClass { Field = new UnspecifiedSubclass() };
            var actual = Roundtrip(expected);

            Assert.AreEqual(SubclassType.B, actual.Subtype);
            Assert.IsInstanceOfType(actual.Field, typeof(SubclassB));
        }
    }
}
