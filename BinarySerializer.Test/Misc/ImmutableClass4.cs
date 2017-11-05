namespace BinarySerialization.Test.Misc
{
    public class ImmutableClass4
    {
        public ImmutableClass4(int header, int? responseId = null)
        {
            Header = header;
            ResponseId = responseId;
        }

        public ImmutableClass4(int header)
        {
            Header = header;
        }

        [FieldOrder(0)]
        public int Header { get; }

        [Ignore]
        public int? ResponseId { get; }
    }
}