namespace BinarySerialization.Test.Length
{
    public class LengthSourceClass : ILengthSource
    {
        [FieldOrder(0)]
        public int Length { get; set; }

        [FieldOrder(1)]
        public InterfaceAncestoryBindingClass Internal { get; set; }
    }
}
