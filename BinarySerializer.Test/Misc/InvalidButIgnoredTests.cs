using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Misc
{
    [TestClass]
    public class InvalidButIgnoredTests : TestBase
    {
        [TestMethod]
        public void InvalidButIgnoredTest()
        {
            Roundtrip(new InvalidButIgnoredContainerClass
            {
                InvalidButIgnored = new InvalidButIgnoredTypeClass()
            });
        }
    }
}
