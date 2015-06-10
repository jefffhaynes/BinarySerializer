namespace BinarySerialization.Test.ItemLength
{
    public class JaggedArrayClass
    {
        [FieldOrder(0)]
        public int[] NameLengths { get; set; }

        [FieldOrder(1)]
        [ItemLength("NameLengths")]
        public string[] Names { get; set; } 
    }
}
