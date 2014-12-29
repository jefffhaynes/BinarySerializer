using System;

namespace BinarySerialization
{
    /// <summary>
    /// Specifies an explicit list termination condition when it is defined as part of an item in the collection.
    /// </summary>
    /// <example>
    /// Consider a list of toys of unknown length, where the toys themselves define a field indicating that
    /// we have reached the end of the collection.
    /// <code>
    /// using BinarySerialization;
    /// 
    /// public class Toy
    /// {
    ///     public string Name { get; set; }
    ///     public bool IsLast { get; set; }
    /// }
    /// 
    /// public class ToyChest
    /// {
    ///     [ItemSerializeUntil("IsLast", true)]
    ///     public List&lt;Toy&gt; Toys { get; set; }
    /// }
    /// 
    /// </code>
    /// </example>
    /// <remarks>
    /// It should be noted that this attributes differs from the <see cref="SerializeUntilAttribute"/> class in
    /// a subtle but significant way.  The <see cref="SerializeUntilAttribute"/> should be used in cases where
    /// the serialization of collection is terminated explicity by something defined outside of the scope of an
    /// item in the collection.  See <see cref="SerializeUntilAttribute"/> for more information.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false)]
    public sealed class ItemSerializeUntilAttribute : FieldBindingBaseAttribute, IConstAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSerializeUntilAttribute"/> class with a
        /// path to the member within the item to be used as a termination condition.
        /// </summary>
        /// <param name="itemValuePath">The path to the member within the item to be used as a
        /// termination condition.</param>
        /// <param name="constValue">The value to use in the termination comparison.</param>
        public ItemSerializeUntilAttribute(string itemValuePath, object constValue)
        {
            ItemValuePath = itemValuePath;
            ConstValue = constValue;
        }

        public string ItemValuePath { get; set; }
		
        /// <summary>
        /// The value to use in the termination comparison.  If the item value referenced in the value path
        /// matches this value, serialization of the collection will be terminated.
        /// </summary>
        public object ConstValue { get; set; }

        /// <summary>
        /// Used to specify whether the terminating item should be included in the collection.
        /// </summary>
        public bool ExcludeLastItem { get; set; }

        internal override bool IsConstSupported
        {
            get { return false; }
        }

        public object GetConstValue()
        {
            return ConstValue;
        }
    }
}
