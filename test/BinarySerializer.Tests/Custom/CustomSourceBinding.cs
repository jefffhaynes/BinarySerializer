namespace BinarySerialization.Test.Custom
{
    public class CustomSourceBinding
    {
        [FieldOrder(0)]
        public Varuint NameLength { get; set; }

        [FieldOrder(1)]
        [FieldLength("NameLength.Value")]
        public string Name { get; set; }

        [Ignore]
        public uint Length => NameLength.Value;
    }
}
