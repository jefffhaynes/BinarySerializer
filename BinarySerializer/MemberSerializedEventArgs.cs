using System;

namespace BinarySerialization
{
    /// <summary>
    /// Provides data for the <see cref="BinarySerializer.MemberSerialized"/> and <see cref="BinarySerializer.MemberDeserialized"/> events.
    /// </summary>
    public class MemberSerializedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the MemberSerializedEventArgs class with the member name and value.
        /// </summary>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="value">The value of the member.</param>
        /// <param name="context">The current serialization context.</param>
        public MemberSerializedEventArgs(string memberName, object value, BinarySerializationContext context)
        {
            MemberName = memberName;
            Value = value;
            Context = context;
        }

        /// <summary>
        /// The name of the member.
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// The member value.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// The current serialization context.
        /// </summary>
        public BinarySerializationContext Context { get; set; }
    }
}
