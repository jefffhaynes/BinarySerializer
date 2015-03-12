using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Binding
{
    [TestClass]
    public class InvalidBindingTests :TestBase
    {
        [TestMethod]
        public void InvalidCollectionBindingSourceTest()
        {
            var expected = new CollectionBindingSourceClass {List = new List<int>(), Name = string.Empty};
            var actual = Roundtrip(expected);
        }
    }
}
