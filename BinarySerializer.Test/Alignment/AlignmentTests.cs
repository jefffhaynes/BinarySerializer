using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Alignment
{
    [TestClass]
    public class AlignmentTests : TestBase
    {
        [TestMethod]
        public void AlignmentTest()
        {
            var actual = RoundtripReverse<AlignmentClass>(new byte[]
            {
                0x2, 0x0, 0x0, 0x0,
                (byte) 'h', (byte) 'i', 0, 0
            });

            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("hi", actual.Value);
        }
    }
}
