using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Encoding
{
    [TestClass]
    public class EncodingTests : TestBase
    {
        [TestMethod]
        public void TestEncoding()
        {
            var expected = new EncodingClass {Name = "غدير"};
            var expectedData = System.Text.Encoding.GetEncoding("windows-1256").GetBytes(expected.Name);
            var actual = Roundtrip(expected, expectedData);

            Assert.AreEqual(expected.Name, actual.Name);
        }
    }
}
