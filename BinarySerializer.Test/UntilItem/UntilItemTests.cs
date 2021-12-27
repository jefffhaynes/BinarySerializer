namespace BinarySerialization.Test.UntilItem;

[TestClass]
public class UntilItemTests : TestBase
{
    [TestMethod]
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

        var expected = new UntilItemContainer { Items = items, ItemsLastItemExcluded = items, BoundItems = items, EnumTerminationItems = items };

        var actual = Roundtrip(expected);

        Assert.AreEqual(expected.Items.Count, actual.Items.Count);
        Assert.AreEqual(expected.ItemsLastItemExcluded.Count - 1, actual.ItemsLastItemExcluded.Count);
    }

    [TestMethod]
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

        var expected = new UntilItemContainer { Items = items, ItemsLastItemExcluded = items, BoundItems = items, EnumTerminationItems = items };

        var actual = Roundtrip(expected);

        Assert.AreEqual(expected.BoundItems.Count, actual.BoundItems.Count);
        Assert.AreEqual(expected.BoundItems[2].LastItem, actual.SerializeUntilField);
    }

    [TestMethod]
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

        Assert.AreEqual(expected.EnumTerminationItems.Count, actual.EnumTerminationItems.Count);
    }

    [TestMethod]
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

        Assert.AreEqual(expected.Sections.Count, actual.Sections.Count);
        Assert.AreEqual(expected.Sections[0].Items.Count, actual.Sections[0].Items.Count);
        Assert.AreEqual(expected.Sections[1].Items.Count, actual.Sections[1].Items.Count);
    }
}
