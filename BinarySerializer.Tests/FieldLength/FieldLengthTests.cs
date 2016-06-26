using BinarySerialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.FieldLength
{
    public class FieldLengthPrimitives
    {
        [FieldLength(6)]
        public string Name { get; set; }

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