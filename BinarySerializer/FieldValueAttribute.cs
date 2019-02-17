namespace BinarySerialization
{
    /// <summary>
    ///     Specifies a value binding for a member or object sub-graph.
    /// </summary>
    public sealed class FieldValueAttribute : FieldValueAttributeBase
    {
        /// <summary>
        ///     Initializes a new instance of the FieldValue class.
        /// </summary>
        public FieldValueAttribute(string valuePath) : base(valuePath)
        {
        }

        /// <summary>
        ///     This is called by the framework to indicate a new operation.
        /// </summary>
        /// <param name="context"></param>
        protected override object GetInitialState(BinarySerializationContext context)
        {
            return context.Value;
        }

        /// <summary>
        ///     This is called one or more times by the framework to add data to the computation.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        protected override object GetUpdatedState(object state, byte[] buffer, int offset, int count)
        {
            return state;
        }

        /// <summary>
        ///     This is called by the framework to retrieve the final value from computation.
        /// </summary>
        /// <returns></returns>
        protected override object GetFinalValue(object state)
        {
            return state;
        }
    }
}