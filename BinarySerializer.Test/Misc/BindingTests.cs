using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Misc
{
    [TestClass]
    public class BindingTests : TestBase
    {
        [TestMethod]
#if TESTASYNC
        [ExpectedException(typeof(AggregateException))]
#else
        [ExpectedException(typeof (InvalidOperationException))]
#endif
        public void InvalidForwardBindingTest()
        {
            Roundtrip(new InvalidForwardBindingClass());
        }
    }
}