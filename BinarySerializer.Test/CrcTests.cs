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
            var crc = new Crc16(Crc16Polynomial, 0xffff);
            TestCrc16(crc, "hello world", 0xefeb);
        }

        [Fact]
        public void Crc16RemainderReflectedTest()
        {
            var crc = new Crc16(Crc16Polynomial, 0xffff) {IsRemainderReflected = true};
            TestCrc16(crc, "hello world", 0xd7f7);
        }

        [Fact]
        public void Crc16DataReflectedTest()
        {
            var crc = new Crc16(Crc16Polynomial, 0xffff) {IsDataReflected = true};
            TestCrc16(crc, "hello world", 0x9f8a);
        }

        [Fact]
        public void Crc16DataReflectedRemainderReflectedTest()
        {
            var crc = new Crc16(Crc16Polynomial, 0xffff) { IsDataReflected = true, IsRemainderReflected = true };
            TestCrc16(crc, "hello world", 0x51f9);
        }

        [Fact]
        public void Crc32Test()
        {
            var crc = new Crc32(Crc32Polynomial, 0xffffffff);
            var messageData = System.Text.Encoding.ASCII.GetBytes("hello world");
            crc.Compute(messageData, 0, messageData.Length);
            var final = crc.ComputeFinal();
            Assert.Equal(0xfd11ac49, final);
        }

        [Fact]
        public void Crc32MultipleCallsTest()
        {
            var crc = new Crc32(Crc32Polynomial, 0xffffffff);
            var messageData = System.Text.Encoding.ASCII.GetBytes("hello");
            crc.Compute(messageData, 0, messageData.Length);
            messageData = System.Text.Encoding.ASCII.GetBytes(" ");
            crc.Compute(messageData, 0, messageData.Length);
            messageData = System.Text.Encoding.ASCII.GetBytes("world");
            crc.Compute(messageData, 0, messageData.Length);
            var final = crc.ComputeFinal();
            Assert.Equal(0xfd11ac49, final);
            final = crc.ComputeFinal();
            Assert.Equal(0xfd11ac49, final);
        }

        [Fact]
        public void Crc32NoDataReflectTest()
        {
            var crc = new Crc32(Crc32Polynomial, 0xffffffff) {IsDataReflected = false};
            var messageData = System.Text.Encoding.ASCII.GetBytes("hello world");
            crc.Compute(messageData, 0, messageData.Length);
            var final = crc.ComputeFinal();
            Assert.Equal(0xf8485336, final);
        }

        [Fact]
        public void Crc32NoRemainderReflectTest()
        {
            var crc = new Crc32(Crc32Polynomial, 0xffffffff) { IsRemainderReflected = false };
            var messageData = System.Text.Encoding.ASCII.GetBytes("hello world");
            crc.Compute(messageData, 0, messageData.Length);
            var final = crc.ComputeFinal();
            Assert.Equal(0x923588bf, final);
        }

        [Fact]
        public void Crc32NoDataReflectNoRemainderReflectTest()
        {
            var crc = new Crc32(Crc32Polynomial, 0xffffffff) { IsDataReflected = false, IsRemainderReflected = false };
            var messageData = System.Text.Encoding.ASCII.GetBytes("hello world");
            crc.Compute(messageData, 0, messageData.Length);
            var final = crc.ComputeFinal();
            Assert.Equal((uint)0x6cca121f, final);
        }

        private void TestCrc16(Crc16 crc, string value, ushort expected)
        {
            var messageData = System.Text.Encoding.ASCII.GetBytes(value);
            crc.Compute(messageData, 0, messageData.Length);
            var final = crc.ComputeFinal();
            Assert.Equal(expected, final);
        }
    }
}