using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Misc
{
    public class DictionaryMemberClass
    {
        public DictionaryMemberClass()
        {
            Field = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Field { get; set; }
    }

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
