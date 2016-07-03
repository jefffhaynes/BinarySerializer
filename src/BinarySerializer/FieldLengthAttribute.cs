﻿using System;

namespace BinarySerialization
{
    /// <summary>
    /// Specifies the length of a member or object subgraph.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FieldLengthAttribute : FieldBindingBaseAttribute, ILengthAttribute, IConstAttribute
    {
        /// <summary>
        /// Initializes a new instance of the FieldLength class.
        /// </summary>
        public FieldLengthAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the FieldLength class with a constant length.
        /// </summary>
        /// <param name="constLength">The fixed-size length of the decorated member.</param>
        public FieldLengthAttribute(ulong constLength)
        {
            ConstLength = constLength;
        }


        /// <summary>
        /// Initializes a new instance of the FieldLength class with a path pointing to a binding source member.
        /// <param name="lengthPath">A path to the source member.</param> 
        /// </summary>
        public FieldLengthAttribute(string lengthPath) : base(lengthPath)
        {
        }

        /// <summary>
        /// The number of items in the decorated member for fixed-sized members or object subgraphs.
        /// </summary>
        public ulong ConstLength { get; set; }

        /// <summary>
        /// Get constant value or null if not constant.
        /// </summary>
        public object GetConstValue()
        {
            return ConstLength;
        }
    }
}
