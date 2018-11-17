using System;
using System.Collections.Generic;
using Xunit;

namespace BinarySerialization.Test.Misc
{
    public class CountTests : TestBase
    {
        [Fact]
        public void ThrowIfMemberImplementsIDictionary()
        {
#if TESTASYNC
            Assert.Throws<AggregateException>(() => Roundtrip(new DictionaryMemberClass()));
#else
            Assert.Throws<InvalidOperationException>(() => Roundtrip(new DictionaryMemberClass()));
#endif

        }

        [Fact]
        public void ThrowIfImplementsIDictionary()
        {
#if TESTASYNC
            Assert.Throws<AggregateException>(() => Roundtrip(new Dictionary<string, string>()));
#else
            Assert.Throws<InvalidOperationException>(() => Roundtrip(new Dictionary<string, string>()));
#endif
        }
    }
}