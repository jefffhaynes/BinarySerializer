using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BinarySerialization.Test.Count
{
        public class CountTests : TestBase
    {
        [Fact]
        public void ConstCountTest()
        {
            var actual = Roundtrip(new ConstCountClass<string>
            {
                Field = new List<string>(TestSequence),
                Field2 = TestSequence.ToArray()
            });

            Assert.Equal(3, actual.Field.Count);
            Assert.Equal(3, actual.Field2.Length);
        }

        [Fact]
        public void PrimitiveConstCountTest()
        {
            var actual = Roundtrip(new ConstCountClass<int>
            {
                Field = new List<int>(PrimitiveTestSequence),
                Field2 = PrimitiveTestSequence.ToArray()
            
            });

            Assert.Equal(3, actual.Field.Count);
            Assert.Equal(3, actual.Field2.Length);
        }

        [Fact]
        public void CountTest()
        {
            var expected = new BoundCountClass
                {
                    Field = new List<string>(TestSequence)
                };

            var actual = Roundtrip(expected);
            Assert.Equal(TestSequence.Length, actual.Field.Count);
            Assert.Equal(TestSequence.Length, actual.FieldCountField);
            Assert.True(expected.Field.SequenceEqual(actual.Field));
        }

        [Fact]
        public void ConstCountMismatchTest()
        {
            var actual = Roundtrip(new ConstCountClass<string> { Field = new List<string>(TestSequence.Take(2)) });
            Assert.Equal(3, actual.Field.Count);
        }

        [Fact]
        public void PrimtiveConstCountMismatchTest()
        {
            var actual = Roundtrip(new ConstCountClass<int>
            {
                Field = new List<int>(PrimitiveTestSequence.Take(2)),
                Field2 = PrimitiveTestSequence.Take(2).ToArray()
            });
            Assert.Equal(3, actual.Field.Count);
        }

        [Fact]
        public void PrimitiveListBindingTest()
        {
            var expected = new PrimitiveListBindingClass { Ints = new List<int> { 1, 2, 3 } };
            var actual = Roundtrip(expected);

            Assert.Equal(expected.Ints.Count, actual.ItemCount);
        }

        [Fact]
        public void PrimitiveArrayBindingTest()
        {
            var expected = new PrimitiveArrayBindingClass {Ints = new[] {1, 2, 3}};
            var actual = Roundtrip(expected);

            Assert.Equal(expected.Ints.Length, actual.ItemCount);
        }

        [Fact]
        public void NullListBindingTest()
        {
            var expected = new PrimitiveListBindingClass();
            var actual = Roundtrip(expected);

            Assert.Null(actual.Ints);
        }

        [Fact]
        public void NullArrayBindingTest()
        {
            var expected = new PrimitiveArrayBindingClass();
            var actual = Roundtrip(expected);

            Assert.Null(actual.Ints);
        }
    }
}
