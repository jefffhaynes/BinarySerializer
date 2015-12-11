using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Length
{
    [TestClass]
    public class LengthTests : TestBase
    {
        [TestMethod]
        public void ConstLengthTest()
        {
            var actual = Roundtrip(new ConstLengthClass {Field = "FieldValue"}, 3);
            Assert.AreEqual("Fie", actual.Field);
        }

        [TestMethod]
        public void NullStringConstLengthTest()
        {
            var actual = Roundtrip(new ConstLengthClass(), 3);
            Assert.AreEqual(string.Empty, actual.Field);
        }

        [TestMethod]
        public void LengthBindingTest()
        {
            var expected = new BoundLengthClass<string> {Field = "FieldValue"};
            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Field.Length, actual.FieldLengthField);
            Assert.AreEqual(expected.Field, actual.Field);
        }

        [TestMethod]
        public void LengthBindingTest2()
        {
            var expected = new BoundLengthClass<byte[]> { Field = System.Text.Encoding.ASCII.GetBytes("FieldValue") };
            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Field.Length, actual.FieldLengthField);
            Assert.IsTrue(expected.Field.SequenceEqual(actual.Field));
        }

        [TestMethod]
        public void CollectionConstLengthTest()
        {
            var expected = new ConstCollectionLengthClass { Field = new List<string>(TestSequence) };
            var actual = Roundtrip(expected);
            Assert.AreEqual(TestSequence.Length, actual.Field.Count);
            Assert.IsTrue(expected.Field.SequenceEqual(actual.Field));
        }

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void CollectionConstLengthMismatchTest()
        //{
        //    var expected = new ConstCollectionLengthClass { Field = new List<string>(TestSequence.Take(2)) };
        //    Roundtrip(expected);
        //}

        [TestMethod]
        public void CollectionLengthTest()
        {
            var expected = new BoundLengthClass<List<string>> { Field = new List<string>(TestSequence) };
            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Field.Count * 2, actual.FieldLengthField);
            Assert.AreEqual(TestSequence.Length, actual.Field.Count);
        }

        [TestMethod]
        public void EmptyCollectionLengthTest()
        {
            var expected = new BoundLengthClass<List<string>> { Field = new List<string>() };
            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Field.Count * 2, actual.FieldLengthField);
            Assert.AreEqual(0, actual.Field.Count);
        }

        [TestMethod]
        public void ComplexFieldLengthTest()
        {
            var expected = new BoundLengthClass<ConstLengthClass>
                {
                    Field = new ConstLengthClass { Field = "FieldValue" }
                };
            var actual = Roundtrip(expected);
            Assert.AreEqual(3, actual.Field.Field.Length);
            Assert.AreEqual(3, actual.FieldLengthField);
        }

        [TestMethod]
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
            Assert.AreEqual(2, actual.Field.Collection.Count);
        }

        [TestMethod]
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

            Assert.AreEqual(expected.InnerClass.Value, actual.InnerClass.Value);
            Assert.AreEqual(expected.InnerClass.Value.Length, actual.InnerClass.ValueLength);
            Assert.AreEqual(expected.InnerClass2.Value, actual.InnerClass2.Value);
            Assert.AreEqual(expected.InnerClass2.Value.Length, actual.InnerClass2.ValueLength);
        }

        [TestMethod]
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

        [TestMethod]
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

            Assert.AreEqual(expected.Items[0].Name, actual.Items[0].Name);
            Assert.AreEqual(expected.Items[1].Name, actual.Items[1].Name);
            Assert.AreEqual(expected.Items[2].Name, actual.Items[2].Name);
        }
    }
}
