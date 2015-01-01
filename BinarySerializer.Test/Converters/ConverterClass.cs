using BinarySerialization;

namespace BinarySerializer.Test.Converters
{
    public class ConverterClass
    {
        [FieldOrder(0)]
        public double HalfFieldLength { get; set; }

        [FieldOrder(1)]
        [FieldLength("HalfFieldLength", ConverterType = typeof(TwiceConverter))]
        public string Field { get; set; }
    }
}