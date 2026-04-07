using System;
using System.Collections.Generic;
using System.Text;

namespace BinarySerialization
{
    /// <summary> Tells the <see cref="BinarySerializer"/> to pack the decorated member. </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PackAttribute : Attribute
    {
        /// <summary> Initializes a new instance of the <see cref="PackAttribute"/> class. </summary>
        public PackAttribute() { }
    }
}
