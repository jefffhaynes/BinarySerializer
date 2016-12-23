using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.ItemSubtype
{
    [TestClass]
    public class ItemSubtypeTests : TestBase
    {
        [TestMethod]
        public void ItemSubtypeTest()
        {
            var expected = new ItemSubtypeClass
            {
                Items = new List<IItemSubtype>
                {
                    new ItemTypeB {Value = 1},
                    new ItemTypeB {Value = 2},
                    new ItemTypeB {Value = 3}
                }
            };

            var actual = Roundtrip(expected, new byte[] {2, 1, 2, 3});

            Assert.AreEqual(2, actual.Indicator);
            Assert.AreEqual(3, actual.Items.Count);
            Assert.AreEqual(expected.Items[0].GetType(), actual.Items[0].GetType());
            Assert.AreEqual(expected.Items[1].GetType(), actual.Items[1].GetType());
            Assert.AreEqual(expected.Items[2].GetType(), actual.Items[2].GetType());
        }

        [TestMethod]
        public void CustomItemSubtypeTest()
        {
            var expected = new ItemSubtypeClass
            {
                Items = new List<IItemSubtype>
                {
                    new CustomItem(),
                    new CustomItem()
                }
            };

            var data = new byte[] {3}.Concat(CustomItem.Data).Concat(CustomItem.Data).ToArray();
            var actual = Roundtrip(expected, data);

            Assert.AreEqual(3, actual.Indicator);
            Assert.AreEqual(2, actual.Items.Count);
            Assert.AreEqual(expected.Items[0].GetType(), actual.Items[0].GetType());
            Assert.AreEqual(expected.Items[1].GetType(), actual.Items[1].GetType());
        }
    }
}
