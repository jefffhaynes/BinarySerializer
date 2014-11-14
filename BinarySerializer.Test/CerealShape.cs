using BinarySerialization;

namespace BinarySerializer.Test
{
    public enum CerealShape
    {
        [SerializeAsEnum("CIR")]
        Circular,

        [SerializeAsEnum("SQR")]
        Square
    }
}
