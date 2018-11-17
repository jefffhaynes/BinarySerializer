﻿using Xunit;

namespace BinarySerialization.Test.Scale
{
    public class ScaledTests : TestBase
    {
        [Fact]
        public void ScaleTest()
        {
            var expected = new ScaledValueClass {Value = 3};
            var actual = Roundtrip(expected, new byte[] {0x6, 0, 0, 0});
            Assert.Equal(expected.Value, actual.Value);
        }

        [Fact]
        public void NegativeScaleTest()
        {
            var expected = new ScaledValueClass { Value = -3 };
            var actual = Roundtrip(expected, new byte[] { 0xFA, 0xFF, 0xFF, 0xFF });
            Assert.Equal(expected.Value, actual.Value);
        }
    }
}
