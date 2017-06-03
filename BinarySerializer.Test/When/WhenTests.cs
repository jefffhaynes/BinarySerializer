using Xunit;

namespace BinarySerialization.Test.When
{
    
    public class WhenTests : TestBase
    {
        [Fact]
        public void WhenStringTest()
        {
            var expected = new WhenStringTestClass
            {
                WhatToDo = "PickOne",
                SerializeThis = 1,
                DontSerializeThis = 2,
                SerializeThisNoMatterWhat = 3
            };

            var actual = Roundtrip(expected);

            Assert.Equal(expected.SerializeThis, actual.SerializeThis);
            Assert.NotEqual(expected.DontSerializeThis, actual.DontSerializeThis);
            Assert.Equal(expected.SerializeThisNoMatterWhat, actual.SerializeThisNoMatterWhat);
        }

        [Fact]
        public void WhenIntTest()
        {
            var expected = new WhenIntTestClass
            {
                WhatToDo = 1,
                SerializeThis = 100,
                DontSerializeThis = 200,
                SerializeThisNoMatterWhat = 300
            };

            var actual = Roundtrip(expected);

            Assert.Equal(expected.SerializeThis, actual.SerializeThis);
            Assert.NotEqual(expected.DontSerializeThis, actual.DontSerializeThis);
            Assert.Equal(expected.SerializeThisNoMatterWhat, actual.SerializeThisNoMatterWhat);
        }

        [Fact]
        public void WhenEnumTest()
        {
            var expected = new WhenEnumTestClass
            {
                WhatToDo = 1,
                SerializeThis = 1000,
                DontSerializeThis = 2000,
                SerializeThisNoMatterWhat = 3000
            };

            var actual = Roundtrip(expected);

            Assert.Equal(expected.SerializeThis, actual.SerializeThis);
            Assert.NotEqual(expected.DontSerializeThis, actual.DontSerializeThis);
            Assert.Equal(expected.SerializeThisNoMatterWhat, actual.SerializeThisNoMatterWhat);
        }

        [Fact]
        public void WhenConverterTest()
        {
            var expected = new WhenConverterTestClass
            {
                WhatToDo = 1,
                SerializeThis = 1000,
                DontSerializeThis = 2000,
                SerializeThisNoMatterWhat = 3000
            };

            var actual = Roundtrip(expected);

            Assert.Equal(expected.SerializeThis, actual.SerializeThis);
            Assert.NotEqual(expected.DontSerializeThis, actual.DontSerializeThis);
            Assert.Equal(expected.SerializeThisNoMatterWhat, actual.SerializeThisNoMatterWhat);
        }
    }
}