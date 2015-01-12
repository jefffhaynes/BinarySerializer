using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Length
{
    [TestClass]
    public class LengthTests : TestBase
    {
        [TestMethod]
        public void ConstLengthTest()
        {
            var actual = Roundtrip(new ConstLengthClass {Field = "FieldValue"});
            Assert.AreEqual("Fie", actual.Field);
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
    }
}
