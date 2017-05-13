namespace BinarySerialization
{
    /// <summary>
    ///     Provides data for the <see cref="BinarySerializer.MemberSerialized" /> and
    ///     <see cref="BinarySerializer.MemberDeserialized" /> events.
    /// </summary>
    public class MemberSerializedEventArgs : MemberSerializingEventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the MemberSerializedEventArgs class with the member name and value.
        /// </summary>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="value">The value of the member.</param>
        /// <param name="context">The current serialization context.</param>
        /// <param name="offset">The current offset in the stream relative to the start of the overall operation.</param>
        public MemberSerializedEventArgs(string memberName, object value, BinarySerializationContext context,
            long offset) : base(memberName, context, offset)
        {
            Value = value;
        }

        /// <summary>
        ///     The member value.
        /// </summary>
        public object Value { get; }
    }
}