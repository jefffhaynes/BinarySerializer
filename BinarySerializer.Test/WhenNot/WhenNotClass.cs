namespace BinarySerialization.Test.WhenNot
{
    public class WhenNotClass
    {
        [FieldOrder(0)]
        public bool ExcludeValue { get; set; }

        [FieldOrder(1)]
        [SerializeWhenNot(nameof(ExcludeValue), true)]
        public int Value { get; set; }
    }
}
