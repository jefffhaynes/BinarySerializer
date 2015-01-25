using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Custom
{
    [TestClass]
    public class CustomTests : TestBase
    {
        [TestMethod]
        public void TestVaruint()
        {
            var expected = new Varuint {Value = ushort.MaxValue};
            var actual = Roundtrip(expected, 3);

            Assert.AreEqual(expected.Value, actual.Value);
        }

        //[TestMethod]
        //public void CustomWithContextTest()
        //{
        //    var expected = new CustomWithContextClass();

        //    var serializer = new BinarySerialization.BinarySerializer();
        //    var stream = new MemoryStream();

        //    serializer.Serialize(stream, expected, "context");
        //}
    }
}
