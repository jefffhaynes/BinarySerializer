using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Count
{
    [TestClass]
    public class CountTests : TestBase
    {
        [TestMethod]
        public void ConstCountTest()
        {
            var actual = Roundtrip(new ConstCountClass<string>
            {
                Field = new List<string>(TestSequence),
                Field2 = TestSequence.ToArray()
            });

            Assert.AreEqual(3, actual.Field.Count);
            Assert.AreEqual(3, actual.Field2.Length);
        }

        [TestMethod]
        public void PrimitiveConstCountTest()
        {
            var actual = Roundtrip(new ConstCountClass<int>
            {
                Field = new List<int>(PrimitiveTestSequence),
                Field2 = PrimitiveTestSequence.ToArray()
            
            });

            Assert.AreEqual(3, actual.Field.Count);
            Assert.AreEqual(3, actual.Field2.Length);
        }

        [TestMethod]
        public void CountTest()
        {
            var expected = new BoundCountClass
                {
                    Field = new List<string>(TestSequence)
                };

            var actual = Roundtrip(expected);
            Assert.AreEqual(TestSequence.Length, actual.Field.Count);
            Assert.AreEqual(TestSequence.Length, actual.FieldCountField);
            Assert.IsTrue(expected.Field.SequenceEqual(actual.Field));
        }

        [TestMethod]
        public void ConstCountMismatchTest()
        {
            var actual = Roundtrip(new ConstCountClass<string> { Field = new List<string>(TestSequence.Take(2)) });
            Assert.AreEqual(3, actual.Field.Count);
        }

        [TestMethod]
        public void PrimtiveConstCountMismatchTest()
        {
            var actual = Roundtrip(new ConstCountClass<int> { Field = new List<int>(PrimitiveTestSequence.Take(2)) });
            Assert.AreEqual(3, actual.Field.Count);
        }

        [TestMethod]
        public void PrimitiveArrayBindingTest()
        {
            var expected = new PrimitiveArrayBindingClass {Ints = new[] {1, 2, 3}};
            var actual = Roundtrip(expected);

            Assert.AreEqual(expected.Ints.Length, actual.ItemCount);
        }
    }
}
