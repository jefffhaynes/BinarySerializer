using System;

namespace BinarySerialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class FieldValueAttribute : FieldBindingBaseAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the FieldValue class with a path pointing to a binding source member.
        ///     <param name="valuePath">A path to the source member.</param>
        /// </summary>
        protected FieldValueAttribute(string valuePath) : base(valuePath)
        {
        }

        public virtual int BlockSize => 4096;

        protected virtual void Reset(object fieldValue)
        {
        }

        protected abstract void Compute(byte[] buffer, int offset, int count);
        
        protected abstract object ComputeFinal();

        internal virtual void ResetInternal(object fieldValue)
        {
            Reset(fieldValue);
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