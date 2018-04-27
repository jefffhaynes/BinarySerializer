namespace BinarySerialization.Test.Encoding
{
    public class FieldEncodingClass
    {
        [FieldOrder(0)]
        public string Encoding { get; set; }

        [FieldOrder(1)]
        [FieldEncoding(nameof(Encoding), typeof(EncodingConverter))]
        public string Value { get; set; }
    }
}
