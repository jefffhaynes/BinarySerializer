using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Misc
{
    [TestClass]
    public class MismatchBindingTests : TestBase
    {
        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void MismatchBindingTest()
        {
            var expected = new MismatchBindingClass {Name1 = "Alice", Name2 = "Bob"};
            Roundtrip(expected);
        }
    }
}