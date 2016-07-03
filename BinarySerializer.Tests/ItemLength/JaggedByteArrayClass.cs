namespace BinarySerialization.Test.ItemLength
{
    public class JaggedByteArrayClass
    {
        [FieldOrder(0)]
        public int NameCount { get; set; }

        [FieldOrder(1)]
        [FieldCount("NameCount")]
        public byte[] NameDataLengths { get; set; }

        [FieldOrder(2)]
        [FieldCount("NameCount")]
        [ItemLength("NameDataLengths")]
        public byte[][] NameData { get; set; }
    }
}