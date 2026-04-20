using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue180
{
    [TestClass]
    public class Issue180Tests : TestBase
    {
        public class BitFieldsAlone
        {
            [FieldOrder(0)]
            [FieldBitLength(7)]
            public byte SevenBits;
            [FieldOrder(1)]
            [FieldBitLength(1)]
            public byte OneBit;
        }

        public class BitFieldsPrecededByByte
        {
            [FieldOrder(0)]
            public byte First;

            [FieldOrder(1)]
            [FieldBitLength(7)]
            public byte SevenBits;

            [FieldOrder(2)]
            [FieldBitLength(1)]
            public byte OneBit;
        }

        [TestMethod]
        public void TestOutOfMemory()
        {
            var maxLength = 7;

            for (int i = 0; i < maxLength; i++)
            {
                Console.WriteLine($"Trying to deserialize {nameof(BitFieldsAlone)} with array size {i}");
                var serialized = new byte[i];
                var _ = Deserialize<BitFieldsAlone>(serialized);
            }

            for (int i = 0; i < maxLength; i++)
            {
                Console.WriteLine($"Trying to deserialize array of {nameof(BitFieldsAlone)} with array size {i}");
                var serialized = new byte[i];
                var _ = Deserialize<BitFieldsAlone[]>(serialized);
            }

            for (int i = 0; i < maxLength; i++)
            {
                Console.WriteLine($"Trying to deserialize {nameof(BitFieldsPrecededByByte)} with array size {i}");
                var serialized = new byte[i];
                var _ = Deserialize<BitFieldsPrecededByByte>(serialized);
            }

            for (int i = 0; i < maxLength; i += 2)
            {
                Console.WriteLine($"Trying to deserialize array of complete {nameof(BitFieldsPrecededByByte)} with array size {i}");
                var serialized = new byte[i];
                var _ = Deserialize<BitFieldsPrecededByByte[]>(serialized);
            }

            for (int i = 0; i < maxLength; i++)
            {
                Console.WriteLine($"Trying to deserialize array of partial {nameof(BitFieldsPrecededByByte)} with array size {i}");
                var serialized = new byte[i];
                // This call will never return and consumes all available memory
                var _ = Deserialize<BitFieldsPrecededByByte[]>(serialized);
            }
        }

        public class ObservationWrapper
        {
            [FieldOrder(1)]
            public ObservationData[] Observations;
        }
        public class ObservationData
        {
            [FieldOrder(0)]
            [FieldCount(1)]
            public byte[] OneByte; // 1
            
            [FieldOrder(1)]
            [FieldBitLength(7)]
            public byte SevenBits; // 67 => 0b1000011
            
            [FieldOrder(2)]
            [FieldBitLength(1)]
            public byte OneBit; // 0 => Eight bit of 67 which is 0
        }

        [TestMethod]
        public void TestInfinityLoop_ShouldNotBeInfinityLoop_ShouldCrash()
        {
            var msg = new byte[]
            {
                1, // 1 Byte which does not fit into the provided type
                67, // OneByteArray
                75, // 7 bits + 1 bit
            };
            var ser = new BinarySerializer();
            Assert.ThrowsException<InvalidOperationException>(() => ser.Deserialize<ObservationWrapper>(msg));
        }

        [TestMethod]
        public void TestInfinityLoop_ShouldNotBeInfinityLoop_ShouldCrashOnAsync()
        {
            var msg = new byte[]
            {
                1, // 1 Byte which does not fit into the provided type
                67, // OneByteArray
                75, // 7 bits + 1 bit
            };
            var ser = new BinarySerializer();
            Assert.ThrowsExceptionAsync<InvalidOperationException>(() => ser.DeserializeAsync<ObservationWrapper>(msg));
        }

        [TestMethod]
        public void TestInfinityLoop_DoesNotEndUpInInfinityLoop()
        {
            var msg = new byte[]
            {
                67, // OneByte
                75, // 7 bits + 1 bit
            };
            var ser = new BinarySerializer();
            var message = ser.Deserialize<ObservationData[]>(msg);

            Assert.IsNotNull(message);
            Assert.AreEqual(67, message[0].OneByte[0]);
            Assert.AreEqual(0, message[0].OneBit);
            Assert.AreEqual(75, message[0].SevenBits);
        }
    }
}
