namespace BinarySerialization.Test.ItemLength
{
    public class JaggedByteArrayClass
    {
        [FieldOrder(0)]
        public int NameCount { get; set; }

        [FieldOrder(1)]
        [FieldCount(nameof(NameCount))]
        public byte[] NameDataLengths { get; set; }

        [FieldOrder(2)]
        [FieldCount(nameof(NameCount))]
        [ItemLength(nameof(NameDataLengths))]
        public byte[][] NameData { get; set; }
    }
}