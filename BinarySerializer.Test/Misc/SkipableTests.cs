using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Misc
{
    [TestClass]
    public class SkipableTests : TestBase
    {
        public void SkipTest()
        {
            var actual = Deserialize<SkipableContainerClass>(new byte[0]);
            Assert.IsNull(actual);
        }
    }
}
