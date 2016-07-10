using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Misc
{
    [TestClass]
    public class BindingTests : TestBase
    {
        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void InvalidForwardBindingTest()
        {
            Roundtrip(new InvalidForwardBindingClass());
        }
    }
}