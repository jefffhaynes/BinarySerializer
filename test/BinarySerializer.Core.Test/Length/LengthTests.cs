using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BinarySerialization.Test.Length
{
        public class LengthTests : TestBase
    {
        [Fact]
        public void ConstLengthTest()
        {
            var actual = Roundtrip(new ConstLengthClass {Field = "FieldValue"}, 6);
            Assert.Equal("Fie", actual.Field);
        }

        [Fact]
        public void NullStringConstLengthTest()
        {
            var actual = Roundtrip(new ConstLengthClass(), 6);
            Assert.Equal(string.Empty, actual.Field);
            Assert.Equal(System.Text.Encoding.ASCII.GetString(new byte[] {0,0,0}), actual.Field2);
        }

        [Fact]
        public void LengthBindingTest()
        {
            var expected = new BoundLengthClass<string> {Field = "FieldValue"};
            var actual = Roundtrip(expected);
            Assert.Equal(expected.Field.Length, actual.FieldLengthField);
            Assert.Equal(expected.Field, actual.Field);
        }

        [Fact]
        public void LengthBindingTest2()
        {
            var expected = new BoundLengthClass<byte[]> { Field = System.Text.Encoding.ASCII.GetBytes("FieldValue") };
            var actual = Roundtrip(expected);
            Assert.Equal(expected.Field.Length, actual.FieldLengthField);
            Assert.True(expected.Field.SequenceEqual(actual.Field));
        }

        [Fact]
        public void CollectionConstLengthTest()
        {
            var expected = new ConstCollectionLengthClass { Field = new List<string>(TestSequence) };
            var actual = Roundtrip(expected);
            Assert.Equal(TestSequence.Length, actual.Field.Count);
            Assert.True(expected.Field.SequenceEqual(actual.Field));
        }

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void CollectionConstLengthMismatchTest()
        //{
        //    var expected = new ConstCollectionLengthClass { Field = new List<string>(TestSequence.Take(2)) };
        //    Roundtrip(expected);
        //}

        [Fact]
        public void CollectionLengthTest()
        {
            var expected = new BoundLengthClass<List<string>> { Field = new List<string>(TestSequence) };
            var actual = Roundtrip(expected);
            Assert.Equal(expected.Field.Count * 2, actual.FieldLengthField);
            Assert.Equal(TestSequence.Length, actual.Field.Count);
        }

        [Fact]
        public void EmptyCollectionLengthTest()
        {
            var expected = new BoundLengthClass<List<string>> { Field = new List<string>() };
            var actual = Roundtrip(expected);
            Assert.Equal(expected.Field.Count * 2, actual.FieldLengthField);
            Assert.Equal(0, actual.Field.Count);
        }

        [Fact]
        public void ComplexFieldLengthTest()
        {
            var expected = new BoundLengthClass<ConstLengthClass>
                {
                    Field = new ConstLengthClass { Field = "FieldValue" }
                };
            var actual = Roundtrip(expected);
            Assert.Equal(3, actual.Field.Field.Length);
            Assert.Equal(6, actual.FieldLengthField);
        }

        [Fact]
        public void ContainedCollectionTest()
        {
            var expected = new BoundLengthClass<ContainedCollection>
                {
                    Field = new ContainedCollection
                        {
                            Collection = new List<string>
                                {
                                    "hello",
                                    "world"
                                }
                        }
                };

            var actual = Roundtrip(expected);
            Assert.Equal(2, actual.Field.Collection.Count);
        }

        [Fact]
        public void PaddedLengthTest()
        {
            var expected = new PaddedLengthClassClass
            {
                InnerClass = new PaddedLengthClassInnerClass
                {
                    Value = "hello"
                },

                InnerClass2 = new PaddedLengthClassInnerClass
                {
                    Value = "world"
                }
            };

            var actual = Roundtrip(expected, 40);

            Assert.Equal(expected.InnerClass.Value, actual.InnerClass.Value);
            Assert.Equal(expected.InnerClass.Value.Length, actual.InnerClass.ValueLength);
            Assert.Equal(expected.InnerClass2.Value, actual.InnerClass2.Value);
            Assert.Equal(expected.InnerClass2.Value.Length, actual.InnerClass2.ValueLength);
        }

        [Fact]
        public void EmbeddedConstrainedCollectionTest()
        {
            var expected = new EmbeddedConstrainedCollectionClass
            {
                Inner = new EmbeddedConstrainedCollectionInnerClass
                {
                    Items = new List<string>
                    {
                        "we",
                        "have",
                        "nothing",
                        "to",
                        "fear"
                    }
                }
            };

            Roundtrip(expected, 10);
        }

        [Fact]
        public void BoundItemTest()
        {
            var expected = new BoundItemContainerClass
            {
                Items = new List<BoundItemClass>
                {
                    new BoundItemClass {Name = "Alice"},
                    new BoundItemClass {Name = "Frank"},
                    new BoundItemClass {Name = "Steve"}
                }
            };

            var actual = Roundtrip(expected);

            Assert.Equal(expected.Items[0].Name, actual.Items[0].Name);
            Assert.Equal(expected.Items[1].Name, actual.Items[1].Name);
            Assert.Equal(expected.Items[2].Name, actual.Items[2].Name);
        }
    }
}
