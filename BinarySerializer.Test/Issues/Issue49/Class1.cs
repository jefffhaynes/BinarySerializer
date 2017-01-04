using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerialization.Test.Issues.Issue49
{
    public class MessageHeader
    {
        [FieldOrder(0)]
        public MessageType MessageType { get; set; }

        [FieldOrder(1)]
        public int MessageLength { get; set; } // Should be the length of the remainder of the message, i.e. the number of bytes that will follow.
    }

    public class RemoteOperationHeader
    {
        [FieldOrder(0)]
        public RemoteOpTypes OpType { get; set; }

        [FieldOrder(1)]
        public int Length { get; set; } // Should be the length of the remainder of the message, i.e. the number of bytes that will follow.
    }

    public class Message : MessageBase
    {
        // Should inherit the members of the base class and then add the next element to the message     
        [FieldOrder(0)]
        public RemoteInvokeAction Action { get; set; }
    }

    public class MessageBase
    {
        [FieldOrder(0)]
        public MessageHeader Header { get; set; }

        [FieldOrder(1)]
        [FieldLength("Header.MessageLength")]
        public MessageContent Content { get; set; }
    }

    public class MessageContent
    {
        [FieldOrder(1)]
        public RemoteOperationHeader RemoteOpHeader { get; set; }

        [FieldOrder(2)]
        [FieldLength("RemoteOpHeader.Length")]
        [Subtype("RemoteOpHeader.OpType", RemoteOpTypes.Invoke, typeof(RemoteInvokeOp))]
        [Subtype("RemoteOpHeader.OpType", RemoteOpTypes.Command, typeof(RemoteInvokeComm))]
        [Subtype("RemoteOpHeader.OpType", RemoteOpTypes.Result, typeof(RemoteInvokeRes))]
        public RemoteOperationBase RemoteOperation { get; set; }
    }

    public abstract class RemoteOperationBase
    {
        
    }

    public class RemoteInvokeOp : RemoteOperationBase
    {
    }

    public class RemoteInvokeComm : RemoteOperationBase
    {
    }

    public class RemoteInvokeRes : RemoteOperationBase
    {
    }
}
