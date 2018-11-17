using System;
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
            Assert.Equal(System.Text.Encoding.ASCII.GetString(new byte[] {0, 0, 0}), actual.Field2);
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
            var expected = new BoundLengthClass<byte[]> {Field = System.Text.Encoding.ASCII.GetBytes("FieldValue")};
            var actual = Roundtrip(expected);
            Assert.Equal(expected.Field.Length, actual.FieldLengthField);
            Assert.True(expected.Field.SequenceEqual(actual.Field));
        }

        [Fact]
        public void CollectionConstLengthTest()
        {
            var expected = new ConstCollectionLengthClass {Field = new List<string>(TestSequence)};
            var actual = Roundtrip(expected);
            Assert.Equal(TestSequence.Length, actual.Field.Count);
            Assert.True(expected.Field.SequenceEqual(actual.Field));
        }

        [Fact]
        public void CollectionLengthTest()
        {
            var expected = new BoundLengthClass<List<string>> {Field = new List<string>(TestSequence)};
            var actual = Roundtrip(expected);
            Assert.Equal(expected.Field.Count*2, actual.FieldLengthField);
            Assert.Equal(TestSequence.Length, actual.Field.Count);
        }

        [Fact]
        public void EmptyCollectionLengthTest()
        {
            var expected = new BoundLengthClass<List<string>> {Field = new List<string>()};
            var actual = Roundtrip(expected);
            Assert.Equal(expected.Field.Count*2, actual.FieldLengthField);
            Assert.Empty(actual.Field);
        }

        [Fact]
        public void ComplexFieldLengthTest()
        {
            var expected = new BoundLengthClass<ConstLengthClass>
            {
                Field = new ConstLengthClass {Field = "FieldValue"}
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

        [Fact]
        public void MultibindingTest()
        {
            var expected = new MultibindingClass {Value = "hi"};
            var actual = Roundtrip(expected, new byte[] {0x02, (byte) 'h', (byte) 'i', 0x02});

            Assert.Equal(2, actual.Length);
            Assert.Equal(2, actual.Length2);
        }

        [Fact]
        public void EmptyClassTest()
        {
            var expected = new EmptyClass {Internal = new EmptyInternalClass()};
            var actual = Roundtrip(expected);

            Assert.NotNull(actual);
            Assert.NotNull(actual.Internal);
        }

        [Fact]

        public void InvalidForwardBindingTest()
        {
#if TESTASYNC
            Assert.Throws<AggregateException>(() => Roundtrip(new InvalidForwardBindingClass()));
#else
            Assert.Throws<InvalidOperationException>(() => Roundtrip(new InvalidForwardBindingClass()));
#endif
        }

        [Fact]
        public void InterfaceAncestoryBindingTest()
        {
            var expected = new LengthSourceClass
            {
                Internal = new InterfaceAncestoryBindingClass
                {
                    Value = "hello"
                }
            };

            var actual = Roundtrip(expected);

            Assert.Equal(expected.Internal.Value, actual.Internal.Value);
        }

        [Fact]
        public void AncestorBindingCollectionItemTest()
        {
            var expected = new AncestorBindingCollectionClass
            {
                Items = new List<AncestorBindingCollectionItemClass>
                {
                    new AncestorBindingCollectionItemClass
                    {
                        Value = "hello"
                    }
                }
            };

            Roundtrip(expected);
        }

        [Fact]
        public void PrimitiveNullByteArrayLengthTest()
        {
            PrimitiveNullArrayLengthTest<byte>();
        }

        [Fact]
        public void PrimitiveNullSByteArrayLengthTest()
        {
            PrimitiveNullArrayLengthTest<sbyte>();
        }

        [Fact]
        public void PrimitiveNullShortArrayLengthTest()
        {
            PrimitiveNullArrayLengthTest<short>();
        }

        [Fact]
        public void PrimitiveNullUShortArrayLengthTest()
        {
            PrimitiveNullArrayLengthTest<ushort>();
        }

        [Fact]
        public void OneWayLengthBindingTest()
        {
            var expected = new OneWayLengthBindingClass {Value = "hi"};
            Roundtrip(expected, new byte[] {0, (byte) 'h', (byte) 'i'});
        }

        private void PrimitiveNullArrayLengthTest<TValue>()
        {
            var expected = new PrimitiveArrayClass<TValue>();
            Roundtrip(expected, 5);
        }
    }
}