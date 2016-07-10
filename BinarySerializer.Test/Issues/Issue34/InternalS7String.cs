namespace BinarySerialization.Test.Issues.Issue34
{
    public class InternalS7String
    {
        public InternalS7String()
        {
        }

        public InternalS7String(string value)
        {
            Value = value;
        }

        [FieldOrder(0)]
        public byte Length { get; set; }

        [FieldOrder(1)]
        [FieldLength("Length")]
        public string Value { get; set; }
    }
}