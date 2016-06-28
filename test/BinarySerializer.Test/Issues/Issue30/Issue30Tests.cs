using Xunit;

namespace BinarySerialization.Test.Issues.Issue30
{
        public class Issue30Tests
    {
        [Fact]
        public void Test()
        {
            MessageSerializer binSerializer = new MessageSerializer();

            var msg = new CommMessage<PayloadA>
                (
                    new Header
                    {
                        Stx = 2,
                        HeaderDef = 1,
                        Behaviour = 3,
                        Security = 0,
                        SenderId = 176,
                        ReceiverId = 161,
                        MsgType = MessageType.Ack,
                        TelegrammId = 100,
                        PayloadSize = 100,
                        PayloadType = PayloadType.INIT
                    },
                    new PayloadA(1, 5)
                );

            var result1 = binSerializer.BinarySerializeMessage(msg);
            var result2 = (CommMessage<PayloadA>)binSerializer.BinaryDeserializeMessage(result1, typeof(CommMessage<PayloadA>));
        }
    }
}
