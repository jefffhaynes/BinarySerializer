using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BinarySerialization.Test.ItemSubtype
{
    
    public class ItemSubtypeTests : TestBase
    {
        [Fact]
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

            Assert.Equal(2, actual.Indicator);
            Assert.Equal(3, actual.Items.Count);
            Assert.Equal(expected.Items[0].GetType(), actual.Items[0].GetType());
            Assert.Equal(expected.Items[1].GetType(), actual.Items[1].GetType());
            Assert.Equal(expected.Items[2].GetType(), actual.Items[2].GetType());
        }

        [Fact]
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

            Assert.Equal(3, actual.Indicator);
            Assert.Equal(2, actual.Items.Count);
            Assert.Equal(expected.Items[0].GetType(), actual.Items[0].GetType());
            Assert.Equal(expected.Items[1].GetType(), actual.Items[1].GetType());
        }


        [Fact]
        public void DefaultItemSubtypeTest()
        {
            var data = new byte[] {4, 0, 1, 2, 3 };
            var actual = Deserialize<ItemSubtypeClass>(data);

            Assert.Equal(4, actual.Indicator);
            Assert.Single(actual.Items);
            Assert.Equal(typeof(DefaultItemType), actual.Items[0].GetType());
        }

        [Fact]
        public void ItemSubtypeFactoryTest()
        {
            var expected = new ItemSubtypeFactoryClass
            {
                Value = new List<IItemSubtype>
                {
                    new ItemTypeB(),
                    new ItemTypeB()
                }
            };

            var actual = Roundtrip(expected);

            Assert.Equal(3, actual.Key);
        }

        [Fact]
        public void ItemSubtypeMixedTest()
        {
            var expected = new ItemSubtypeMixedClass
            {
                Value = new List<IItemSubtype>
                {
                    new ItemTypeB(),
                    new ItemTypeB()
                }
            };

            var actual = Roundtrip(expected);

            Assert.Equal(2, actual.Key);
        }

        [Fact]
        public void ItemSubtypeFactoryWithDefaultTest()
        {
            var data = new byte[] { 4, 0, 1, 2, 3 };
            var actual = Deserialize<ItemSubtypeFactoryWithDefaultClass>(data);

            Assert.Equal(4, actual.Key);
            Assert.Single(actual.Items);
            Assert.Equal(typeof(DefaultItemType), actual.Items[0].GetType());
        }
    }
}
