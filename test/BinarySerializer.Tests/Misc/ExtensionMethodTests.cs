
using System.Linq;
using BinarySerialization;
using Xunit;
using System.Reflection;

namespace BinarySerialization.Test.Misc
{
        public class ExtensionMethodTests
    {
        [Fact]
        public void GetAttributeTest()
        {   
            var type = typeof(Chemical);
            var member = type.GetTypeInfo().GetMember("Formula").Single();
            var attribute = member.GetCustomAttribute<SerializeAsAttribute>();
            Assert.NotNull(attribute);
        }
    }
}
