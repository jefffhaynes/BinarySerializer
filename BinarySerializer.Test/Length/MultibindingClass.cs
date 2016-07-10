namespace BinarySerialization.Test.Length
{
    public class MultibindingClass
    {
        [FieldOrder(0)]
        public byte Length { get; set; }

        [FieldOrder(1)]
        [FieldLength("Length")]
        [FieldLength("Length2", BindingMode = BindingMode.OneWay)]
        public string Value { get; set; }

        [FieldOrder(2)]
        public byte Length2 { get; set; }
    }
}
