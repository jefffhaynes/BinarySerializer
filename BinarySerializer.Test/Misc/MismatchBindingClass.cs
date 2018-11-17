namespace BinarySerialization.Test.Misc
{
    public class MismatchBindingClass
    {
        [FieldOrder(0)]
        public int NameLength { get; set; }

        [FieldOrder(1)]
        [FieldLength("NameLength")]
        public string Name1 { get; set; }

        [FieldOrder(2)]
        [FieldLength("NameLength")]
        public string Name2 { get; set; }
    }
}