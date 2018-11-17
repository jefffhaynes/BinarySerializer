namespace BinarySerialization.Test
{
    public enum CerealShape
    {
        [SerializeAsEnum("CIR")] Circular,

        [SerializeAsEnum("SQR")] Square
    }
}