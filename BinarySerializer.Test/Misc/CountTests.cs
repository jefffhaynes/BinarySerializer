using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Misc
{
    [TestClass]
    public class CountTests : TestBase
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ThrowIfMemberImplementsIDictionary()
        {
            Roundtrip(new DictionaryMemberClass());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ThrowIfImplementsIDictionary()
        {
            Roundtrip(new Dictionary<string, string>());
        }
    }
}