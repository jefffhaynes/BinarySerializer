using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.When
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
                SerializeThisNoMatterWhat = 3,
                WhatToDo2 = 1,
                SerializeThis2 = 50,
                DontSerializeThis2 = 100,
                SerializeThisNoMatterWhat2 = 200
            };

            var actual = Roundtrip(expected);

            Assert.AreEqual(expected.SerializeThis, actual.SerializeThis);
            Assert.AreNotEqual(expected.DontSerializeThis, actual.DontSerializeThis);
            Assert.AreEqual(expected.SerializeThisNoMatterWhat, actual.SerializeThisNoMatterWhat);
            Assert.AreEqual(expected.SerializeThis2, actual.SerializeThis2);
            Assert.AreNotEqual(expected.DontSerializeThis2, actual.DontSerializeThis2);
            Assert.AreEqual(expected.SerializeThisNoMatterWhat2, actual.SerializeThisNoMatterWhat2);
        }
    }
}
