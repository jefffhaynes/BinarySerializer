
using System.Linq;
using BinarySerialization;
using Xunit;

namespace BinarySerialization.Test.Misc
{
        public class ExtensionMethodTests
    {
        [Fact]
        public void GetAttributeTest()
        {
            var type = typeof(Chemical);
            var member = type.GetMember("Formula").Single();
            var attribute = member.GetAttribute<SerializeAsAttribute>();
            Assert.NotNull(attribute);
        }
    }
}
