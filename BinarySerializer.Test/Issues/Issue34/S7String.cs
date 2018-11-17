namespace BinarySerialization.Test.Issues.Issue34
{
    public class S7String
    {
        [FieldOrder(0)]
        public byte MaxLength { get; set; }

        [FieldOrder(1)]
        [FieldLength("MaxLength", ConverterType = typeof (AdditionConverter), ConverterParameter = 1)]
        public InternalS7String Value { get; set; }
    }
}