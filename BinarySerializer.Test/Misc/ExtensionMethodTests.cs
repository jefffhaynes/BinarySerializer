using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace BinarySerialization.Test.Misc
{
    [TestClass]
    public class ExtensionMethodTests
    {
        [TestMethod]
        public void GetAttributeTest()
        {
            var type = typeof (Chemical);
            var member = type.GetTypeInfo().GetMember("Formula").Single();
            var attribute = member.GetAttribute<SerializeAsAttribute>();
            Assert.IsNotNull(attribute);
        }
    }
}