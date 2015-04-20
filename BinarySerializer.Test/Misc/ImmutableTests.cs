using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Misc
{
    [TestClass]
    public class ImmutableTests : TestBase
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PrivateSetterTest()
        {
            var expected = new PrivateSetterClass();
            Roundtrip(expected);
        }

        [TestMethod]
        public void ImmutableTest()
        {
            var expected = new ImmutableClass(3, 4);
            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Value, actual.Value);
        }
    }
}
