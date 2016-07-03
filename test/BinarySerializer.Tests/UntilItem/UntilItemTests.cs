using System.Collections.Generic;
using Xunit;

namespace BinarySerialization.Test.UntilItem
{
        public class UntilItemTests : TestBase
    {
        [Fact]
        public void UntilItemConstTest()
        {
            var items = new List<UntilItemClass>
            {
                new UntilItemClass
                {
                    Name = "Alice",
                    LastItem = "Nope",
                    Description = "She's just a girl in the world"
                },
                new UntilItemClass
                {
                    Name = "Bob",
                    LastItem = "Not yet",
                    Description = "Well, he's just this guy, you know?"
                },
                new UntilItemClass
                {
                    Name = "Charlie",
                    LastItem = "Yep",
                    Description = "What??  That's a great idea!"
                }
            };

            var expected = new UntilItemContainer {Items = items, ItemsLastItemExcluded = items, BoundItems = items};

            var actual = Roundtrip(expected);

            Assert.Equal(expected.Items.Count, actual.Items.Count);
            Assert.Equal(expected.ItemsLastItemExcluded.Count - 1, actual.ItemsLastItemExcluded.Count);
        }

        [Fact]
        public void UntilItemBoundTest()
        {
            var items = new List<UntilItemClass>
            {
                new UntilItemClass
                {
                    Name = "Alice",
                    LastItem = "Nope",
                    Description = "She's just a girl in the world"
                },
                new UntilItemClass
                {
                    Name = "Bob",
                    LastItem = "Not yet",
                    Description = "Well, he's just this guy, you know?"
                },
                new UntilItemClass
                {
                    Name = "Charlie",
                    LastItem = "Yep",
                    Description = "What??  That's a great idea!"
                }
            };

            var expected = new UntilItemContainer { Items = items, ItemsLastItemExcluded = items, BoundItems = items };

            var actual = Roundtrip(expected);

            Assert.Equal(expected.BoundItems.Count, actual.BoundItems.Count);
            Assert.Equal(expected.BoundItems[2].LastItem, actual.SerializeUntilField);
        }
    }
}
