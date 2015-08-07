using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Custom
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

        [TestMethod]
        public void CustomWithContextTest()
        {
            var expected = new CustomWithContextClass();

            var serializer = new BinarySerializer();
            var stream = new MemoryStream();

            serializer.Serialize(stream, expected, "context");
        }

        [TestMethod]
        public void CustomSourceBindingTest()
        {
            var expected = new CustomSourceBinding {Name = "alice"};
            Roundtrip(expected);
        }
    }
}
