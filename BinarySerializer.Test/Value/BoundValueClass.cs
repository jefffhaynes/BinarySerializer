using BinarySerialization;

namespace BinarySerializer.Test.Value
{
    public class BoundValueClass
    {
        public int ValueField { get; set; }

        [FieldValue("ValueField")]
        public int Field { get; set; }
    }
}