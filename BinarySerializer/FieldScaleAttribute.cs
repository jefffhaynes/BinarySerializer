using System;

namespace BinarySerialization
{
    /// <summary>
    /// Specifies a scaling value for a value-type field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FieldScaleAttribute : FieldBindingBaseAttribute, IConstAttribute
    {
        /// <summary>
        /// Initializes a new instance of the FieldScale attribute with a fixed scaling value.
        /// </summary>
        /// <param name="scale"></param>
        public FieldScaleAttribute(double scale)
        {
            ConstScale = scale;
        }

        /// <summary>
        /// Initializes a new instance of the FieldScale attribute with a path pointing to a source binding member.
        /// </summary>
        /// <param name="path"></param>
        public FieldScaleAttribute(string path) : base(path)
        {
        }

        public double ConstScale { get; set; }

        public object GetConstValue()
        {
            return ConstScale;
        }
    }
}
