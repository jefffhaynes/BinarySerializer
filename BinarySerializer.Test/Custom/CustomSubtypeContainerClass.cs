namespace BinarySerialization.Test.Custom
{
    public class CustomSubtypeContainerClass
    {
        [FieldOrder(0)]
        public byte Indicator { get; set; }

        [FieldOrder(1)]
        [Subtype("Indicator", 1, typeof(CustomSubtypeCustomClass))]
        public CustomSubtypeBaseClass Inner { get; set; }
    }
}
