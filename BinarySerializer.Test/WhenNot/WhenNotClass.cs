namespace BinarySerialization.Test.WhenNot
{
    public class WhenNotClass
    {
        [FieldOrder(0)]
        public bool ExcludeValue { get; set; }

        [FieldOrder(1)]
#pragma warning disable 612, 618
        [SerializeWhenNot(nameof(ExcludeValue), true)]
#pragma warning restore 612, 618
        public int Value { get; set; }
    }
}
