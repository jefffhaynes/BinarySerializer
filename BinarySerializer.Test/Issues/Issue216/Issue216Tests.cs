using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue216
{
    [TestClass]
    public class Issue216Tests : TestBase
    {
        public class PreviewWithComputedSize
        {
            [FieldOrder(0)]
            public uint ResolutionX { get; set; }

            [FieldOrder(1)]
            [FieldLength(4)]
            [SerializeAs(SerializedType.TerminatedString)]
            public string Mark { get; set; }

            [FieldOrder(2)]
            public uint ResolutionY { get; set; }

            [Ignore]
            public uint DataSize => ResolutionX * ResolutionY * 2;

            [FieldOrder(3)]
            [FieldCount(nameof(DataSize))]
            public byte[] Data { get; set; }

            [FieldOrder(4)]
            public byte Tail { get; set; }
        }

        [TestMethod]
        public void ComputedIgnoredPropertyCanBeUsedAsFieldCountSourceDuringDeserialize()
        {
            var data = new byte[]
            {
                0x02, 0x00, 0x00, 0x00,
                (byte)'x', 0x00, 0x00, 0x00,
                0x01, 0x00, 0x00, 0x00,
                0x10, 0x11, 0x12, 0x13,
                0xAA
            };

            var actual = Deserialize<PreviewWithComputedSize>(data);

            Assert.AreEqual((uint)2, actual.ResolutionX);
            Assert.AreEqual("x", actual.Mark);
            Assert.AreEqual((uint)1, actual.ResolutionY);
            CollectionAssert.AreEqual(new byte[] { 0x10, 0x11, 0x12, 0x13 }, actual.Data);
            Assert.AreEqual((byte)0xAA, actual.Tail);
        }
    }
}
