
namespace BinarySerialization.Test.Binding
{
    public class PreviousBindingClassItem
    {
        [FieldOrder(0)]
        public int NextLength { get; set; }

        [FieldOrder(1)]
        [FieldLength("NextLength", RelativeSourceMode = RelativeSourceMode.Previous)]
        public string Value { get; set; }
    }
}
