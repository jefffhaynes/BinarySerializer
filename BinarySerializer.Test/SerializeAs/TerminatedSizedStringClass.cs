namespace BinarySerialization.Test.SerializeAs
{
    class TerminatedSizedStringClass
    {
        [FieldLength(5)]
        [SerializeAs(SerializedType.TerminatedSizedString, StringTerminator = '\n', PaddingValue = 0x0D)]
        public string Value { get; set; }
    }
}
