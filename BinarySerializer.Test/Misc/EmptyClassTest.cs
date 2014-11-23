using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Misc
{
    [TestClass]
    public class EmptyClassTest : TestBase
    {
        [TestMethod]
        public void RoundtripEmptyClassTest()
        {
            Roundtrip(new EmptyClass());
        }
    }
}