namespace BinarySerialization.Test.Ignore
{
    public class IgnoreSubclassContainer
    {
        [FieldOrder(0)]
        public int Indicator { get; set; }

        [FieldOrder(1)]
        [Subtype("Indicator", 1, typeof(IgnoreSubclassClass))]
        public IgnoreSubclassBaseClass Base { get; set; }
    }
}
