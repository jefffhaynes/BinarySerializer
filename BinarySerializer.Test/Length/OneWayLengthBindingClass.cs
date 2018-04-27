namespace BinarySerialization.Test.Length
{
    public class OneWayLengthBindingClass
    {
        [FieldOrder(0)]
        public byte Length { get; set; }

        [FieldOrder(1)]
        [FieldLength(nameof(Length), BindingMode = BindingMode.OneWay)]
        public string Value { get; set; }
    }
}
