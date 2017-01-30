using BinarySerialization;
using BinarySerialization.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.FieldLength
{
    public class FieldLengthPrimitives
    {
        [FieldOrder(0)]
        [FieldLength(6)]
        public string Name { get; set; }

        [FieldOrder(1)]
        [FieldLength(4)]
        [SerializeAs(SerializedType.SizedString)]
        public CerealShape Shape { get; set; }
    }

    [TestClass]
    public class FieldLengthTests : TestBase
    {
        [TestMethod]
        public void RoundtripPrimitives()
        {
            var fieldLengthPrimitives = Roundtrip(new FieldLengthPrimitives
                {
                    Name = "12345678",
                    Shape = CerealShape.Square
                });

            Assert.AreEqual("123456", fieldLengthPrimitives.Name);
            Assert.AreEqual(CerealShape.Square, fieldLengthPrimitives.Shape);
        }
    }
}