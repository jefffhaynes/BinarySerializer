using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.UntilItem
{
    [TestClass]
    public class UntilItemTests : TestBase
    {
        [TestMethod]
        public void UntilItemTest()
        {
            var expected = new UntilItemContainer
            {
                Items = new List<UntilItemClass>
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
                }
            };

            var actual = Roundtrip(expected);

            Assert.AreEqual(expected.Items.Count, actual.Items.Count);
        }
    }
}
