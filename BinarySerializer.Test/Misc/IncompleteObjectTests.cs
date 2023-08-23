using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Misc
{
    [TestClass]
    public class IncompleteObjectTests
    {
        public class SimpleClass
        {
            [FieldOrder(0)]
            public int A { get; set; }

            [FieldOrder(1)]
            public int B { get; set; }
        }

        [TestMethod]
        public void IncompleteObjectTest()
        {
            var serializer = new BinarySerializer
            {
                Options = SerializationOptions.AllowIncompleteObjects
            };

            var actual = serializer.Deserialize<SimpleClass>(new byte[] { 0x1, 0x2, 0x3, 0x4 });
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.B);
        }
    }
}
