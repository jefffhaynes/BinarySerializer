using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Until
{
    [TestClass]
    public class UntilTests : TestBase
    {
        [TestMethod]
        public void TestUntilConst()
        {
            var expected = new UntilTestClass { Items = new List<string> { "unless", "someone", "like", "you" }, AfterItems = "a whole awful lot" };
            var actual = Roundtrip(expected);

            Assert.AreEqual(expected.Items.Count, actual.Items.Count);
            Assert.AreEqual(expected.AfterItems, actual.AfterItems);
        }
    }
}
