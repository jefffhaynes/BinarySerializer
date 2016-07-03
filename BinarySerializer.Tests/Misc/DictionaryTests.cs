using System;
using System.Collections.Generic;
using Xunit;

namespace BinarySerialization.Test.Misc
{
        public class DictionaryTests : TestBase
    {
        [Fact]
        //[ExpectedException(typeof(InvalidOperationException))]
        public void ThrowIfMemberImplementsIDictionary()
        {
            var e = Record.Exception(() => Roundtrip(new DictionaryMemberClass()));
            Assert.NotNull(e);
            Assert.IsType<InvalidOperationException>(e);
        }

        [Fact]
        //[ExpectedException(typeof(InvalidOperationException))]
        public void ThrowIfImplementsIDictionary()
        {
            var e = Record.Exception(() => Roundtrip(new Dictionary<string, string>()));
            Assert.NotNull(e);
            Assert.IsType<InvalidOperationException>(e);
        }
    }
}