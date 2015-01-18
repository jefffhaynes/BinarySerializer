using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Custom
{
    [TestClass]
    public class CustomTests : TestBase
    {
        [TestMethod]
        public void TestVaruint()
        {
            var expected = new Varuint {Value = ushort.MaxValue};
            var actual = Roundtrip(expected, 3);

            Assert.AreEqual(expected.Value, actual.Value);
        }
    }
}
