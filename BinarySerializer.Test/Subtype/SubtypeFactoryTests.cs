using Xunit;

namespace BinarySerialization.Test.Subtype
{
    
    public class SubtypeFactoryTests : TestBase
    {
        [Fact]
        public void SubtypeFactoryTest()
        {
            var expected = new SubtypeFactoryClass
            {
                Value = new SubclassB()
            };

            var actual = Roundtrip(expected);
            Assert.Equal(2, actual.Key);
            Assert.Equal(expected.Value.GetType(), actual.Value.GetType());
        }

        [Fact]
        public void SubtypeMixedTest()
        {
            var expected = new SubtypeMixedClass
            {
                Value = new SubclassB()
            };

            var actual = Roundtrip(expected);
            Assert.Equal(2, actual.Key);
            Assert.Equal(expected.Value.GetType(), actual.Value.GetType());
        }

        [Fact]
        public void SubtypeMixedTest2()
        {
            var expected = new SubtypeMixedClass
            {
                Value = new SubSubclassC()
            };

            var actual = Roundtrip(expected);
            Assert.Equal(3, actual.Key);
            Assert.Equal(expected.Value.GetType(), actual.Value.GetType());
        }

        [Fact]
        public void SubtypeFactoryWithDefaultTest()
        {
            var data = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };
            var actual = Deserialize<SubtypeFactoryWithDefaultClass>(data);
            Assert.Equal(typeof(DefaultSubtypeClass), actual.Value.GetType());
        }

        [Fact]
        public void SubtypeMixedWithDefaultTest()
        {
            var data = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };
            var actual = Deserialize<SubtypeMixedWithDefaultClass>(data);
            Assert.Equal(typeof(DefaultSubtypeClass), actual.Value.GetType());
        }
    }
}
