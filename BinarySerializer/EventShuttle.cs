using System;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class EventShuttle
    {
        /// <summary>
        ///     Occurrs after a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberSerialized;

        /// <summary>
        ///     Occurrs after a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializedEventArgs> MemberDeserialized;

        /// <summary>
        ///     Occurrs before a member has been serialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberSerializing;

        /// <summary>
        ///     Occurrs before a member has been deserialized.
        /// </summary>
        public event EventHandler<MemberSerializingEventArgs> MemberDeserializing;

        public void OnMemberSerialized(ValueNode sender, string name, object value, BinarySerializationContext context)
        {
            var handle = MemberSerialized;
            if(handle != null)
                handle(sender, new MemberSerializedEventArgs(name, value, context));
        }

        public void OnMemberDeserialized(ValueNode sender, string name, object value, BinarySerializationContext context)
        {
            var handle = MemberDeserialized;
            if (handle != null)
                handle(sender, new MemberSerializedEventArgs(name, value, context));
        }

        public void OnMemberSerializing(ValueNode sender, string name, BinarySerializationContext context)
        {
            var handle = MemberSerializing;
            if (handle != null)
                handle(sender, new MemberSerializingEventArgs(name, context));
        }

        public void OnMemberDeserializing(ValueNode sender, string name, BinarySerializationContext context)
        {
            var handle = MemberDeserializing;
            if (handle != null)
                handle(sender, new MemberSerializingEventArgs(name, context));
        }
    }
}
