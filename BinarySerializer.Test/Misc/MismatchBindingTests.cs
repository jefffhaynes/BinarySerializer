using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Misc
{
    [TestClass]
    public class MismatchBindingTests : TestBase
    {
        [TestMethod]
        public void MismatchBindingTest()
        {
            var expected = new MismatchBindingClass {Name1 = "Alice", Name2 = "Bob"};

#if TESTASYNC
            Assert.ThrowsException<AggregateException>(() => Roundtrip(expected));
#else
            Assert.ThrowsException<InvalidOperationException>(() => Roundtrip(expected));
#endif
        }
    }
}