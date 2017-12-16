using System.Collections.Generic;
using Xunit;

namespace BinarySerialization.Test.Issues.Issue61
{
    
    public class Issue61Tests : TestBase
    {
        [Fact]
        public void ListOfObjectsTest()
        {
            var expected = new List<Message>
            {
                new Message
                {
                    Data = new byte[] {0x0, 0x0, 0x0, 0x0}
                }
            };

            var actual = Roundtrip(expected);
            Assert.Single(actual);
        }
    }
}
