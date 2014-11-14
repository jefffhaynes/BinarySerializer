using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BinarySerialization;

namespace BinarySerializer.Test.ItemLength
{
    public class ItemLengthClass
    {
        [ItemLength(3)]
        public List<string> List { get; set; } 
    }

    [TestClass]
    public class ItemLengthTests : TestBase
    {
        [TestMethod]
        public void ConstItemLengthTest()
        {
            var expected = new ItemLengthClass {List = new List<string>(new[] {"abc", "def", "ghi"})};
            var actual = Roundtrip(expected, expected.List.Count*3);
            Assert.IsTrue(expected.List.SequenceEqual(actual.List));
        }
    }
}
