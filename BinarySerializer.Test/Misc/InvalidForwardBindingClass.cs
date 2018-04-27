namespace BinarySerialization.Test.Misc
{
    public class InvalidForwardBindingClass
    {
        [FieldOrder(0)]
        [FieldLength(nameof(Length))]
        public string Value { get; set; }

        [FieldOrder(1)]
        public byte Length { get; set; }
    }
}