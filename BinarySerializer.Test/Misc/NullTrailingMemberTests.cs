using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Misc
{
    [TestClass]
    public class NullTrailingMemberTests : TestBase
    {
        [TestMethod]
        public void NullTrailingMemberTest()
        {
            var container = new NullTrailingMemberClassContainer();

            Roundtrip(container, 12);

            container.Inner.OptionalParameter = 5;

            Roundtrip(container, 13);
        }
    }
}