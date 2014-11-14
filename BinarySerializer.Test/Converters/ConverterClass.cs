using BinarySerialization;

namespace BinarySerializer.Test.Converters
{
    public class ConverterClass
    {
        public double HalfFieldLength { get; set; }

        [FieldLength("HalfFieldLength", ConverterType = typeof(TwiceConverter))]
        public string Field { get; set; }
    }
}