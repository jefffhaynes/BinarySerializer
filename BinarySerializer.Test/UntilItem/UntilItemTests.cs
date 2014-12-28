using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.UntilItem
{
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
                    LastItem = "Yep",
                    Description = "What??  That's a great idea!"
                }
            };

            var expected = new UntilItemContainer {Items = items, ItemsLastItemExcluded = items};

            var actual = Roundtrip(expected);

            Assert.AreEqual(expected.Items.Count, actual.Items.Count);
            Assert.AreEqual(expected.ItemsLastItemExcluded.Count - 1, actual.ItemsLastItemExcluded.Count);
        }
    }
}
