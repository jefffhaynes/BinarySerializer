using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Value
{
    [TestClass]
    public class ValueTests : TestBase
    {
        [TestMethod]
        public void BoundValueTest()
        {
            var expected = new BoundValueClass {ValueField = 1};
            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.ValueField, actual.Field);
        }
    }
}
