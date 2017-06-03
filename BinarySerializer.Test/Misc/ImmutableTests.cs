using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace BinarySerialization.Test.Misc
{
    
    public class ImmutableTests : TestBase
    {
        [Fact]

        public void PrivateSetterTest()
        {
            var expected = new PrivateSetterClass();
#if TESTASYNC
            Assert.Throws<AggregateException>(() => Roundtrip(expected));
#else
            Assert.Throws<InvalidOperationException>(() => Roundtrip(expected));
#endif
        }

        [Fact]
        public void ImmutableTest()
        {
            var expected = new ImmutableClass(3, 4);
            var actual = Roundtrip(expected);
            Assert.Equal(expected.Value, actual.Value);
        }

        [Fact]
        public void ImmutableTest2()
        {
            var expected = new ImmutableClass2(3, 4);
            var actual = Roundtrip(expected);
            Assert.Equal(expected.Value, actual.Value);
        }

        [Fact]
        public void ImmutableWithNullableParametersTest()
        {
            var expected = new ImmutableClass3(33);
            var actual = Roundtrip(expected);
            Assert.Equal(expected.Value, actual.Value);
        }

        [Fact]
        public void ImmutableWithNullableParametersAndIgnoreTest()
        {
            var expected = new ImmutableClass4(4, 5);
            var actual = Roundtrip(expected);
            Assert.Equal(expected.Header, actual.Header);
            Assert.Null(actual.ResponseId);
        }

        [Fact]
        public void ImmutableNoPublicConstructorTest()
        {
            var stream = new MemoryStream(new[] {(byte) 0x1});
            Assert.Throws<InvalidOperationException>(() => Serializer.Deserialize<ImmutableNoPublicConstructorClass>(stream));
        }

        [Fact]
        public void ImmutableItemsList()
        {
            var expected = new List<ImmutableClass>
            {
                new ImmutableClass(1, 2),
                new ImmutableClass(3, 4)
            };

            var actual = Roundtrip(expected);

            Assert.Equal(expected[0].Value, actual[0].Value);
            Assert.Equal(expected[0].Value2, actual[0].Value2);
            Assert.Equal(expected[1].Value, actual[1].Value);
            Assert.Equal(expected[1].Value2, actual[1].Value2);
        }
    }
}