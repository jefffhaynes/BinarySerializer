using System;

namespace BinarySerialization
{
    /// <summary>
    /// Specifies an explicit list termination condition when it is defined as part of an item in the collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ItemSerializeUntilAttribute : FieldBindingBaseAttribute, IConstAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSerializeUntilAttribute"/> class with a
        /// path to the member within the item to be used as a termination condition.
        /// </summary>
        /// <param name="itemValuePath">The path to the member within the item to be used as a
        /// termination condition.</param>
        /// <param name="constValue">The value to use in the termination comparison.</param>
        public ItemSerializeUntilAttribute(string itemValuePath, object constValue = null)
        {
            ItemValuePath = itemValuePath;
            ConstValue = constValue;
        }

        /// <summary>
        /// Gets or sets the path to the binding source property inside the child item.
        /// </summary>
        public string ItemValuePath { get; set; }
		
        /// <summary>
        /// The value to use in the termination comparison.  If the item value referenced in the value path
        /// matches this value, serialization of the collection will be terminated.
        /// </summary>
        public object ConstValue { get; set; }
        
        /// <summary>
        /// Used to specify whether the terminating item should be included in the collection, discarded, 
        /// or whether processing of the underlying data should be deferred.
        /// </summary>
        public LastItemMode LastItemMode { get; set; }

        /// <summary>
        /// Get constant value or null if not constant.
        /// </summary>
        public object GetConstValue()
        {
            return ConstValue;
        }
    }
}
