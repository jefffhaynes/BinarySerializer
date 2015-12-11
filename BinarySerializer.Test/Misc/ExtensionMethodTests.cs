
using System.Linq;
using BinarySerialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Misc
{
    [TestClass]
    public class ExtensionMethodTests
    {
        [TestMethod]
        public void GetAttributeTest()
        {
            var type = typeof(Chemical);
            var member = type.GetMember("Formula").Single();
            var attribute = member.GetAttribute<SerializeAsAttribute>();
            Assert.IsNotNull(attribute);
        }
    }
}
