using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BinarySerialization;

namespace BinarySerializer.Test.ItemLength
{
    public class ItemConstLengthClass
    {
        [ItemLength(3)]
        public List<string> List { get; set; } 
    }

    public class ItemBoundLengthClass
    {
        public int Count { get; set; }

        [ItemLength("Count")]
        public List<string> List { get; set; } 
    }


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
    }
}
