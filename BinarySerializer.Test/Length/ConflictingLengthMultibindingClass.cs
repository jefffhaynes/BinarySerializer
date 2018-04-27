namespace BinarySerialization.Test.Length
{
    public class ConflictingLengthMultibindingClass
    {
        [FieldOrder(0)]
        public byte Length { get; set; }

        [FieldOrder(1)]
        [FieldLength(nameof(Length))]
        [FieldLength(nameof(Length), ConverterType = typeof(RoundUpConverter),
            ConverterParameter = 4, BindingMode = BindingMode.OneWay)]
        public string Value { get; set; }

        [FieldOrder(2)]
        public byte TrailingValue { get; set; }
    }
}
