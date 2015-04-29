using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Ignore
{
    [TestClass]
    public class IgnoreTests : TestBase
    {
        [TestMethod]
        public void IgnoreObjectTest()
        {
            var expected = new IgnoreObjectClass {IgnoreMe = "hello"};
            var actual = Roundtrip(expected);
            Assert.IsNull(actual.IgnoreMe);
        }
    }
}
