using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.ItemLength
{
    [TestClass]
    public class ItemLengthTests : TestBase
    {
        [TestMethod]
        public void ItemConstLengthTest()
        {
            var expected = new ItemConstLengthClass {List = new List<string>(new[] {"abc", "def", "ghi"})};
            var actual = Roundtrip(expected, expected.List.Count*3);
            Assert.IsTrue(expected.List.SequenceEqual(actual.List));
        }

        [TestMethod]
        public void ItemBoundLengthTest()
        {
            var expected = new ItemBoundLengthClass { Items = new List<string>(new[] { "abc", "def", "ghi" }) };

            var itemLength = expected.Items[0].Length;
            var expectedLength = sizeof (int) + itemLength*expected.Items.Count;
            var actual = Roundtrip(expected, expectedLength);

            Assert.AreEqual(itemLength, actual.ItemLength);
            Assert.IsTrue(expected.Items.SequenceEqual(actual.Items));
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ItemBoundMismatchLengthTest_ShouldThrowInvalidOperation()
        {
            var expected = new ItemBoundLengthClass { Items = new List<string>(new[] { "abc", "defghi"}) };
            Roundtrip(expected);
        }

        [TestMethod]
        public void ItemLengthListOfByteArraysTest()
        {
            var expected = new ItemLengthListOfByteArrayClass
            {
                Arrays = new List<byte[]> {new byte[3], new byte[3], new byte[3]}
            };

            var actual = Roundtrip(expected);

            Assert.AreEqual(expected.Arrays.Count, actual.Arrays.Count);
        }

        [TestMethod]
        public void LimitedItemLengthTest()
        {
            var expected = new LimitedItemLengthClassClass
            {
                InnerClasses = new List<LimitedItemLengthClassInnerClass>
                {
                    new LimitedItemLengthClassInnerClass {Value = "hello"},
                    new LimitedItemLengthClassInnerClass {Value = "world"}
                }
            };

            var expectedData = System.Text.Encoding.ASCII.GetBytes("he\0wo\0");
            var actual = Roundtrip(expected, expectedData);

            Assert.AreEqual(expected.InnerClasses[0].Value.Substring(0, 2), actual.InnerClasses[0].Value);
        }
    }
}
