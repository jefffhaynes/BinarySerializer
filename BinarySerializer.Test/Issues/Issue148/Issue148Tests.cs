using System.ComponentModel;
using Xunit;

namespace BinarySerialization.Test.Issues.Issue148
{
    public class Issue148Tests : TestBase
    {
        [Fact]
        public void RoundtripTest()
        {
            var expected = new ProtocolClass
            {
                Header = new HeaderClass
                {
                    ITEM1 = 1,
                    ITEM2 = 0x7f,
                    ITEM3 = 1,
                    ITEM4 = 0x7
                }
            };

            var actual = Roundtrip(expected, new byte[] {0xff, 0x0f});

            Assert.Equal(expected.Header.ITEM1, actual.Header.ITEM1);
            Assert.Equal(expected.Header.ITEM2, actual.Header.ITEM2);
            Assert.Equal(expected.Header.ITEM3, actual.Header.ITEM3);
            Assert.Equal(expected.Header.ITEM4, actual.Header.ITEM4);
            Assert.Equal(expected.Body, actual.Body);
        }

        [Fact]
        public void ProtocolClassTest()
        {
            var bytes = new byte[] {0xff, 0x0f};
            BinarySerializer serializer = new BinarySerializer();
            var protocol = serializer.Deserialize<ProtocolClass>(bytes);
            Assert.Equal(0x01, protocol.Header.ITEM1);
            Assert.Equal(0x7f, protocol.Header.ITEM2);
            Assert.Equal(0x01, protocol.Header.ITEM3);
            Assert.Equal(0x07, protocol.Header.ITEM4);
        }
    }
}