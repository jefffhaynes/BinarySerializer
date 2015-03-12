using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Misc
{
    [TestClass]
    public class AsbtractClassTests : TestBase
    {
        [TestMethod]
        public void AbstractClassTest()
        {
            var container = new AbstractClassContainer {Content = new DerivedClass()};
            Roundtrip(container, 4);
        }
    }
}
