using System;

namespace BinarySerialization
{
    /// <summary>
    /// Used as the abstract base for deriving field value attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class FieldValueAttributeBase : FieldBindingBaseAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the FieldValue class with a path pointing to a binding source member.
        ///     <param name="valuePath">A path to the source member.</param>
        /// </summary>
        protected FieldValueAttributeBase(string valuePath) : base(valuePath)
        {
        }

        /// <summary>
        /// Override to indicate to the framework the expected input block size for this attribute.
        /// </summary>
        public virtual int BlockSize => 4096;

        /// <summary>
        /// This is called by the framework to indicate a new operation.
        /// </summary>
        /// <param name="context"></param>
        protected abstract void Reset(BinarySerializationContext context);

        /// <summary>
        /// This is called one or more times by the framework to add data to the computation.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        protected abstract void Compute(byte[] buffer, int offset, int count);

        /// <summary>
        /// This is called by the framework to retreive the final value from computation.
        /// </summary>
        /// <returns></returns>
        protected abstract object ComputeFinal();

        internal virtual void ResetInternal(BinarySerializationContext context)
        {
            Reset(context);
        }

        internal void ComputeInternal(byte[] buffer, int offset, int count)
        {
            Compute(buffer, offset, count);
        }

        internal object ComputeFinalInternal()
        {
            return ComputeFinal();
        }
    }
}