using System;

namespace BinarySerialization
{
    /// <summary>
    ///     Provides data for the <see cref="BinarySerializer.MemberSerializing" /> and
    ///     <see cref="BinarySerializer.MemberDeserializing" /> events.
    /// </summary>
    public class MemberSerializingEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the MemberSerializingEventArgs class with the member name.
        /// </summary>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="context">The current serialization context.</param>
        /// <param name="offset">The current offset in the stream relative to the start of the overall operation.</param>
        /// <param name="localOffset">The current ojbect-local offset in the stream.</param>
        public MemberSerializingEventArgs(string memberName, BinarySerializationContext context, FieldLength offset, FieldLength localOffset)
        {
            MemberName = memberName;
            Context = context;
            Offset = offset;
            LocalOffset = localOffset;
        }

        /// <summary>
        ///     The name of the member.
        /// </summary>
        public string MemberName { get; }

        /// <summary>
        ///     The current serialization context.
        /// </summary>
        public BinarySerializationContext Context { get; }

        /// <summary>
        ///     The global location in the stream relative to the initial operation.
        /// </summary>
        public FieldLength Offset { get; }

        /// <summary>
        /// The object-local location in the stream relative to the parent object.
        /// </summary>
        public FieldLength LocalOffset { get; }
    }
}