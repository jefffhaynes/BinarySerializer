using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Misc
{
    [TestClass]
    public class CollectionMiscTests : TestBase
    {
        [TestMethod]
        public void ListAtRootTest()
        {
            var expected = new List<string> {"1", "2", "3"};
            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public void ArrayAtRootTest()
        {
            var expected = new[] {"a", "b", "c"};
            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Length, actual.Length);
        }
    }
}
