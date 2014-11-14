using System;

namespace BinarySerialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class FieldValueAttribute : FieldBindingBaseAttribute
    {
        public FieldValueAttribute()
        {
        }

        public FieldValueAttribute(string valuePath)
            : base(valuePath)
        {
        }

        internal override bool IsConstSupported
        {
            get { return false; }
        }
    }
}
