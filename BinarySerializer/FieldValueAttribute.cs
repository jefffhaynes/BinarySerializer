namespace BinarySerialization
{
    public class FieldValueAttribute : FieldValueAttributeBase
    {
        private object _value;

        public FieldValueAttribute(string valuePath) : base(valuePath)
        {
        }

        protected override void Reset(object fieldValue)
        {
            _value = fieldValue;
        }

        protected override void Compute(byte[] buffer, int offset, int count)
        {
        }

        protected override object ComputeFinal()
        {
            return _value;
        }
    }
}
