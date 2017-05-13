using System;
using BinarySerialization.Graph.ValueGraph;

namespace BinarySerialization
{
    internal class EventShuttle
    {
        public bool HasSerializationSubscribers => MemberSerializing != null || MemberSerialized != null;

        public bool HasDeserializationSubscribers => MemberDeserializing != null || MemberDeserialized != null;

        /// <summary>
        ///     Occurs after a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberSerialized;

        /// <summary>
        ///     Occurs after a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberDeserialized;

        /// <summary>
        ///     Occurs before a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberSerializing;

        /// <summary>
        ///     Occurs before a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberDeserializing;

        public void OnMemberSerialized(ValueNode sender, string name, object value, BinarySerializationContext context,
            long offset)
        {
            var handle = MemberSerialized;
            handle?.Invoke(sender, new MemberSerializedEventArgs(name, value, context, offset));
        }

        public void OnMemberDeserialized(ValueNode sender, string name, object value,
            BinarySerializationContext context, long offset)
        {
            var handle = MemberDeserialized;
            handle?.Invoke(sender, new MemberSerializedEventArgs(name, value, context, offset));
        }

        public void OnMemberSerializing(ValueNode sender, string name, BinarySerializationContext context, long offset)
        {
            var handle = MemberSerializing;
            handle?.Invoke(sender, new MemberSerializingEventArgs(name, context, offset));
        }

        public void OnMemberDeserializing(ValueNode sender, string name, BinarySerializationContext context,
            long offset)
        {
            var handle = MemberDeserializing;
            handle?.Invoke(sender, new MemberSerializingEventArgs(name, context, offset));
        }
    }
}