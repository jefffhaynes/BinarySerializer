using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue160
{
    [TestClass]
    public class Issue160Tests : TestBase
    {
        public interface ITest
        {
        }

        public class TestA : ITest
        {
            [FieldOrder(0)]
            [SerializeAs(SerializedType.UInt1)]
            public byte Value1 { get; set; }

            [FieldOrder(1)]
            [SerializeAs(SerializedType.UInt1)]
            public byte Value2 { get; set; }
        }

        public class TestB : ITest
        {
            [FieldOrder(0)]
            [SerializeAs(SerializedType.UInt2)]
            public ushort Value { get; set; }
        }

        public class ItemSubtypeClassWithoutDefault
        {
            [FieldOrder(0)]
            [SerializeAs(SerializedType.UInt1)]
            public byte Indicator { get; set; }

            [FieldOrder(1)]
            [ItemSubtype(nameof(Indicator), 1, typeof(TestA))]
            [ItemSubtype(nameof(Indicator), 2, typeof(TestB))]
            public List<ITest> Items { get; set; }
        }

        [TestMethod]
        [Timeout(2000)]
        public void DeserializeBadBytesWithoutItemSubtypeDefaultDoesNotHang()
        {
            var testBytes = Enumerable.Repeat((byte)0xFF, 5).ToArray();

            var actual = Deserialize<ItemSubtypeClassWithoutDefault>(testBytes);

            Assert.IsNotNull(actual);
            Assert.AreEqual(0xFF, actual.Indicator);
        }
    }
}
