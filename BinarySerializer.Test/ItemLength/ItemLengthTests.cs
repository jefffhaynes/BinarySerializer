using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BinarySerialization.Test.ItemLength
{
    
    public class ItemLengthTests : TestBase
    {
        [Fact]
        public void ItemConstLengthTest()
        {
            var expected = new ItemConstLengthClass {List = new List<string>(new[] {"abc", "def", "ghi"})};
            var actual = Roundtrip(expected, expected.List.Count*3);
            Assert.True(expected.List.SequenceEqual(actual.List));
        }

        [Fact]
        public void ItemBoundLengthTest()
        {
            var expected = new ItemBoundLengthClass {Items = new List<string>(new[] {"abc", "def", "ghi"})};

            var itemLength = expected.Items[0].Length;
            var expectedLength = sizeof (int) + itemLength*expected.Items.Count;
            var actual = Roundtrip(expected, expectedLength);

            Assert.Equal(itemLength, actual.ItemLength);
            Assert.True(expected.Items.SequenceEqual(actual.Items));
        }

        [Fact]
        public void ArrayItemBoundLengthTest()
        {
            var expected = new ArrayItemBoundLengthClass {Items = new[] {"abc", "def", "ghi"}};

            var actual = Roundtrip(expected);

            Assert.Equal(expected.Items.Length, actual.ItemLength);
            Assert.Equal(expected.Items.Length, actual.Items.Length);
        }

        [Fact]
        public void ItemBoundMismatchLengthTest_ShouldThrowInvalidOperation()
        {
            var expected = new ItemBoundLengthClass {Items = new List<string>(new[] {"abc", "defghi"})};
            Assert.Throws<InvalidOperationException>(() => Roundtrip(expected));
        }

        [Fact]
        public void ItemLengthListOfByteArraysTest()
        {
            var expected = new ItemLengthListOfByteArrayClass
            {
                Arrays = new List<byte[]> {new byte[3], new byte[3], new byte[3]}
            };

            var actual = Roundtrip(expected);

            Assert.Equal(expected.Arrays.Count, actual.Arrays.Count);
        }

        [Fact]
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

            Assert.Equal(expected.InnerClasses[0].Value.Substring(0, 2), actual.InnerClasses[0].Value);
        }

        [Fact]
        public void JaggedArrayTest()
        {
            var expected = new JaggedArrayClass {NameArray = new[] {"Alice", "Bob", "Charlie"}};

            var actual = Roundtrip(expected);

            var nameLengths = expected.NameArray.Select(name => name.Length);
            Assert.True(nameLengths.SequenceEqual(actual.NameLengths));
            Assert.True(expected.NameArray.SequenceEqual(actual.NameArray));
        }

        [Fact]
        public void JaggedListTest()
        {
            var expected = new JaggedListClass {NameList = new[] {"Alice", "Bob", "Charlie"}.ToList()};
            var actual = Roundtrip(expected);

            var nameLengths = expected.NameList.Select(name => name.Length);
            Assert.True(nameLengths.SequenceEqual(actual.NameLengths));
            Assert.True(expected.NameList.SequenceEqual(actual.NameList));
        }

        [Fact]
        public void JaggedDoubleBoundTest()
        {
            var expected = new JaggedDoubleBoundClass {NameArray = new[] {"Alice", "Bob", "Charlie"}};
            expected.NameList = expected.NameArray.ToList();

            var actual = Roundtrip(expected);

            var nameLengths = expected.NameArray.Select(name => name.Length);
            Assert.True(nameLengths.SequenceEqual(actual.NameLengths));
            Assert.True(expected.NameArray.SequenceEqual(actual.NameArray));
            Assert.True(expected.NameList.SequenceEqual(actual.NameList));
        }

        [Fact]
        public void JaggedByteArrayTest()
        {
            var names = new[] {"Alice", "Bob", "Charlie"};
            var expected = new JaggedByteArrayClass
            {
                NameData = names.Select(name => System.Text.Encoding.ASCII.GetBytes(name)).ToArray()
            };

            var actual = Roundtrip(expected);

            var actualNames = actual.NameData.Select(nameData => System.Text.Encoding.ASCII.GetString(nameData));
            Assert.True(names.SequenceEqual(actualNames));
        }

        [Fact]
        public void JaggedIntArrayTest()
        {
            var expected = new JaggedIntArrayClass
            {
                Arrays = new[] {new[] {1}, new[] {2, 2}, new[] {3, 3, 3}}
            };

            var actual = Roundtrip(expected);

            Assert.True(expected.Arrays[0].SequenceEqual(actual.Arrays[0]));
            Assert.True(expected.Arrays[1].SequenceEqual(actual.Arrays[1]));
            Assert.True(expected.Arrays[2].SequenceEqual(actual.Arrays[2]));
        }
    }
}