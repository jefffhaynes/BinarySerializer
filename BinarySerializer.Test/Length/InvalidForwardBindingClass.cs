namespace BinarySerialization.Test.Length
{
    public class InvalidForwardBindingClass
    {
        [FieldOrder(0)]
        [FieldLength("Length")]
        public string Value { get; set; }

        [FieldOrder(1)]
        public int Length { get; set; }
    }
}
