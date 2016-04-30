namespace BinarySerialization
{
    /// <summary>
    /// Specifies a value binding for a member or object subgraph.
    /// </summary>
    public sealed class FieldValueAttribute : FieldValueAttributeBase
    {
        private object _value;

        /// <summary>
        /// Initializes a new instance of the FieldValue class.
        /// </summary>
        public FieldValueAttribute(string valuePath) : base(valuePath)
        {
        }

        /// <summary>
        /// This is called by the framework to indicate a new operation.
        /// </summary>
        /// <param name="fieldValue"></param>
        protected override void Reset(object fieldValue)
        {
            _value = fieldValue;
        }

        /// <summary>
        /// This is called one or more times by the framework to add data to the computation.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        protected override void Compute(byte[] buffer, int offset, int count)
        {
        }

        /// <summary>
        /// This is called by the framework to retreive the final value from computation.
        /// </summary>
        /// <returns></returns>
        protected override object ComputeFinal()
        {
            return _value;
        }
    }
}
