using BinarySerialization.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Issues.Issue57
{
    [TestClass]
    public class Issue57Tests : TestBase
    {
        [TestMethod]
        public void Roundtrip()
        {
            var expected = new BigEndianFloatClass
            {
                Value = -48.651363f
            };

            var actual = Roundtrip(expected, sizeof(float));
            Assert.AreEqual(expected.Value, actual.Value);
        }
    }
}
