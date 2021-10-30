using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.SerializeAs
{
    [TestClass]
    public class SerializeAsTest : TestBase
    {
        [TestMethod]
        public void SerializeIntAsSizedStringTest()
        {
            var expected = new SizedStringClass<int> {Value = 33};
            var actual = Roundtrip(expected, System.Text.Encoding.UTF8.GetBytes(expected.Value.ToString()));

            Assert.AreEqual(expected.Value, actual.Value);
        }

        [TestMethod]
        public void SerializeAsLengthPrefixedStringTest()
        {
            var expected = new LengthPrefixedStringClass {Value = new string('c', ushort.MaxValue)};
            var actual = Roundtrip(expected, ushort.MaxValue + 3);

            Assert.AreEqual(expected.Value, actual.Value);
        }

        [TestMethod]
        public void SerializeAsWithPaddingValue()
        {
            var expected = new PaddingValueClass { Value = "hi" };
            var actual = Roundtrip(expected, new byte[] { 0x68, 0x69, 0x33, 0x33, 0x33 });

            Assert.AreEqual(expected.Value, actual.Value.Trim((char) 0x33));
        }

        [TestMethod]
        public void CollectionPaddingValue()
        {
            var expected = new CollectionPaddingValue
            {
                Items = new List<string>
                {
                    "a", "b"
                }
            };

            var actual = Roundtrip(expected, new[]{(byte) 'a', (byte)' ',(byte) 'b', (byte)' '});

            var actualItems = actual.Items.Select(i => i.Trim()).ToList();
            CollectionAssert.AreEqual(expected.Items, actualItems);
        }
    }
}