namespace BinarySerialization.Test.ItemLength
{
    public class JaggedArrayClass
    {
        [FieldOrder(0)]
        public int NameCount { get; set; }

        [FieldOrder(1)]
        [FieldCount("NameCount")]
        public int[] NameLengths { get; set; }

        [FieldOrder(2)]
        [FieldCount("NameCount")]
        [ItemLength("NameLengths")]
        public string[] NameArray { get; set; }
    }
}
