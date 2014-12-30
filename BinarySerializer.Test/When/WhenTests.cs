using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.When
{
    [TestClass]
    public class WhenTests : TestBase
    {
        [TestMethod]
        public void WhenTest()
        {
            var expected = new WhenTestClass
            {
                WhatToDo = "PickOne",
                SerializeThis = 1,
                DontSerializeThis = 2,
                SerializeThisNoMatterWhat = 3
            };

            var actual = Roundtrip(expected);

            Assert.AreEqual(expected.SerializeThis, actual.SerializeThis);
            Assert.AreNotEqual(expected.DontSerializeThis, actual.DontSerializeThis);
            Assert.AreEqual(expected.SerializeThisNoMatterWhat, actual.SerializeThisNoMatterWhat);
        }
    }
}
