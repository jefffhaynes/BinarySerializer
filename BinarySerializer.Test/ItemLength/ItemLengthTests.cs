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
            var expected = new ItemBoundLengthClass { List = new List<string>(new[] { "abc", "def", "ghi" }) };
            var actual = Roundtrip(expected, 4 + expected.List.Count * 3);
            Assert.IsTrue(expected.List.SequenceEqual(actual.List));
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ItemBoundMismatchLengthTest()
        {
            var expected = new ItemBoundLengthClass { List = new List<string>(new[] { "abc", "defghi"}) };
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
