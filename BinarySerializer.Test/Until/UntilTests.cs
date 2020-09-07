using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Until
{
    [TestClass]
    public class UntilTests : TestBase
    {
        [TestMethod]
        public void TestUntilConst()
        {
            var expected = new UntilTestClass<string>
            {
                Items = new List<string> {"unless", "someone", "like", "you"},
                AfterItems = "a whole awful lot"
            };
            var actual = Roundtrip(expected);

            Assert.AreEqual(expected.Items.Count, actual.Items.Count);
            Assert.AreEqual(expected.AfterItems, actual.AfterItems);
        }

        [TestMethod]
        public void PrimitiveTestUntilConst()
        {
            var expected = new UntilTestClass<int> {Items = new List<int> {3, 2, 1}, AfterItems = "a whole awful lot"};
            var actual = Roundtrip(expected);

            Assert.AreEqual(expected.Items.Count, actual.Items.Count);
            Assert.AreEqual(expected.AfterItems, actual.AfterItems);
        }
    }
}