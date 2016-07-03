using Xunit;

namespace BinarySerialization.Test
{
        public class CrcTests
    {
        private static readonly ushort Crc16Polynomial = 0x1021;
        private static readonly uint Crc32Polynomial = 0xedb88320;

        [Fact]
        public void Crc16Test()
        {
            Assert.True(false, "Crc16Test Not Yet Implemented");
            
            //var crc = new Crc16(Crc16Polynomial, 0xffff);
            //TestCrc16(crc, "hello world", 0xefeb);
        }

        [Fact]
        public void Crc16RemainderReflectedTest()
        {

            Assert.True(false, "CrcRemainderReflected Not Yet implmented");
            //var crc = new Crc16(Crc16Polynomial, 0xffff) {IsRemainderReflected = true};
            //TestCrc16(crc, "hello world", 0xd7f7);
        }

        [Fact]
        public void Crc16DataReflectedTest()
        {
            Assert.True(false, "CRC16DataReflect Not Yet implmented");

            //var crc = new Crc16(Crc16Polynomial, 0xffff) { IsDataReflected = true };
            //TestCrc16(crc, "hello world", 0x9f8a);
        }

        [Fact]
        public void Crc32Test()
        {
            Assert.True(false,"Crc32Test Not Implmented");
            //var crc = new Crc32(Crc32Polynomial, 0xffffffff);
            //var messageData = System.Text.Encoding.ASCII.GetBytes("hello world");
            //crc.Compute(messageData, 0, messageData.Length);
            //var final = crc.ComputeFinal();
            //Assert.Equal(0xfd11ac49, final);
        }

        //private void TestCrc16(Crc16 crc, string value, ushort expected)
        //{
        //    var messageData = System.Text.Encoding.ASCII.GetBytes(value);
        //    crc.Compute(messageData, 0, messageData.Length);
        //    var final = crc.ComputeFinal();
        //    Assert.Equal(expected, final);
        //}
    }
}
