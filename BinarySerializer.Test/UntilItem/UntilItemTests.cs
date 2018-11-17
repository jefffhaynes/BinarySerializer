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
                    LastItem = "Yep", // this is only needed for bound case but it's that or reproduce it a bunch of times
                    Description = "What??  That's a great idea!",
                    Type = UntilItemEnum.End
                }
            };

            var expected = new UntilItemContainer {Items = items, ItemsLastItemExcluded = items, BoundItems = items, EnumTerminationItems = items};

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
                    Description = "What??  That's a great idea!",
                    Type = UntilItemEnum.End
                }
            };

            var expected = new UntilItemContainer {Items = items, ItemsLastItemExcluded = items, BoundItems = items, EnumTerminationItems = items};

            var actual = Roundtrip(expected);

            Assert.Equal(expected.BoundItems.Count, actual.BoundItems.Count);
            Assert.Equal(expected.BoundItems[2].LastItem, actual.SerializeUntilField);
        }

        [Fact]
        public void UntilItemEnumTest()
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
                    Description = "What??  That's a great idea!",
                    Type = UntilItemEnum.End
                }
            };

            var expected = new UntilItemContainer { Items = items, ItemsLastItemExcluded = items, BoundItems = items, EnumTerminationItems = items };

            var actual = Roundtrip(expected);

            Assert.Equal(expected.EnumTerminationItems.Count, actual.EnumTerminationItems.Count);
        }

        [Fact]
        public void UntilItemDeferredTest()
        {
            var expected = new UntilItemContainerDeferred
            {
                Sections = new List<Section>
                {
                    new Section
                    {
                        Header = new UntilItemSimpleClass {Type = UntilItemEnum.Header},
                        Items = new List<UntilItemSimpleClass>
                        {
                            new UntilItemSimpleClass(),
                            new UntilItemSimpleClass()
                        }
                    },
                    new Section
                    {
                        Header = new UntilItemSimpleClass {Type = UntilItemEnum.Header},
                        Items = new List<UntilItemSimpleClass>
                        {
                            new UntilItemSimpleClass(),
                            new UntilItemSimpleClass()
                        }
                    },
                }
            };

            var actual = Roundtrip(expected, new byte[]
            {
                2,0,0,0,0,0,
                2,0,0,0,0,0
            });

            Assert.Equal(expected.Sections.Count, actual.Sections.Count);
            Assert.Equal(expected.Sections[0].Items.Count, actual.Sections[0].Items.Count);
            Assert.Equal(expected.Sections[1].Items.Count, actual.Sections[1].Items.Count);
        }
    }
}