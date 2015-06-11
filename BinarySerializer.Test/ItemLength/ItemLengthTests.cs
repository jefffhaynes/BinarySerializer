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
        public void ArrayItemBoundLengthTest()
        {
            var expected = new ArrayItemBoundLengthClass {Items = new[] {"abc", "def", "ghi"}};

            var actual = Roundtrip(expected);

            Assert.AreEqual(expected.Items.Length, actual.ItemLength);
            Assert.AreEqual(expected.Items.Length, actual.Items.Length);
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

        [TestMethod]
        public void JaggedArrayTest()
        {
            var expected = new JaggedArrayClass {NameArray = new[] {"Alice", "Bob", "Charlie"}};
            
            var actual = Roundtrip(expected);

            var nameLengths = expected.NameArray.Select(name => name.Length);
            Assert.IsTrue(nameLengths.SequenceEqual(actual.NameLengths));
            Assert.IsTrue(expected.NameArray.SequenceEqual(actual.NameArray));
        }

        [TestMethod]
        public void JaggedListTest()
        {
            var expected = new JaggedListClass { NameList = new[] { "Alice", "Bob", "Charlie" }.ToList() };
            var actual = Roundtrip(expected);

            var nameLengths = expected.NameList.Select(name => name.Length);
            Assert.IsTrue(nameLengths.SequenceEqual(actual.NameLengths));
            Assert.IsTrue(expected.NameList.SequenceEqual(actual.NameList));
        }

        [TestMethod]
        public void JaggedDoubleBoundTest()
        {
            var expected = new JaggedDoubleBoundClass() { NameArray = new[] { "Alice", "Bob", "Charlie" } };
            expected.NameList = expected.NameArray.ToList();

            var actual = Roundtrip(expected);

            var nameLengths = expected.NameArray.Select(name => name.Length);
            Assert.IsTrue(nameLengths.SequenceEqual(actual.NameLengths));
            Assert.IsTrue(expected.NameArray.SequenceEqual(actual.NameArray));
            Assert.IsTrue(expected.NameList.SequenceEqual(actual.NameList));
        }

        [TestMethod]
        public void JaggedByteArrayTest()
        {
            var names = new[] {"Alice", "Bob", "Charlie"};
            var expected = new JaggedByteArrayClass()
            {
                NameData = names.Select(name => System.Text.Encoding.ASCII.GetBytes(name)).ToArray()
            };

            var actual = Roundtrip(expected);

            Assert.IsTrue(expected.NameData.SequenceEqual(actual.NameData));
        }
    }
}
