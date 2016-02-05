namespace BinarySerialization.Test.SerializeAs
{
    public class TerminatedStringClass
    {
        public string NullTerminatedValue { get; set; }

        [SerializeAs(SerializedType.TerminatedString, Terminator = ';')]
        public string SemicolonTerminatedValue { get; set; }
    }
}
