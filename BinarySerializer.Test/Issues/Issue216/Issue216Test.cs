using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue216
{
    [TestClass]
    public class Issue216Tests : TestBase
    {
        [TestMethod]
        public void TestIssue216()
        {
            var expected = new Preview
            {
                ResolutionX = 2,
                ResolutionY = 3,
                Data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 },
                Empty = 0,
            };

            var actual = Roundtrip(expected, new byte[] { 2, 0, 0, 0, 3, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0, 0 });
            Assert.AreEqual(expected.ResolutionX, actual.ResolutionX);
            Assert.AreEqual(expected.ResolutionY, actual.ResolutionY);
            Assert.AreEqual(expected.Data.Length, actual.Data.Length);
            for (int i = 0; i < expected.Data.Length; i++)
                Assert.AreEqual(expected.Data[i], actual.Data[i]);
        }
    }
}
