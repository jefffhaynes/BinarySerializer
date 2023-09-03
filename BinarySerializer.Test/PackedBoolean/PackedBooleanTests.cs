using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinarySerialization.Test.PackedBoolean
{
    [TestClass, TestCategory("Packed Booleans")]
    public class PackedBooleanTests : TestBase
    {
        [TestMethod]
        public void PreservesData()
        {
            var original = new ValidPackedBooleanClass
            {
                BooleanArray = GenerateBools().Take(50).ToArray()
            };

            var deserialized = Roundtrip(original);

            CheckSequence(original.BooleanArray, deserialized.BooleanArray);
        }

        [TestMethod]
        public void BindsCorrectData()
        {
            var test = new ValidPackedBooleanClass
            {
                BooleanArray = GenerateBools().Take(50).ToArray()
            };

            test = Roundtrip(test);

            Assert.AreEqual(50, test.BooleanArrayCount, "Incorrect count binding.");
            Assert.AreEqual(7, test.BooleanArrayLength, "Incorrect length binding.");
        }

        [TestMethod]
        public void ProperlyPacksBooleans()
        {
            var original = new ValidPackedBooleanClass
            {
                BooleanArray = Enumerable.Repeat(true, 10).ToArray()
            };

            // Count = 10L
            // Length = 2L
            // Packed Booleans = 1111 1111 0000 0011 (Little Endian)
            byte[] expected = new byte[] { 10, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0xFF, 0x03 };
            byte[] result = Serialize(original);

            CheckSequence(expected, result);
        }

        [TestMethod]
        public void RespectsEndianness()
        {
            var original = new EndianAwarePackedBooleanClass
            {
                LittleEndianArray = new[] { true, true, true, true },
                BigEndianArray = new[] { true, true, true, true }
            };

            // Little: 00001111
            // Big: 11110000

            byte[] result = Serialize(original);

            Assert.AreEqual(0x0F, result[0], "Incorrect Little-Endian boolean packing");
            Assert.AreEqual(0xF0, result[1], "Incorrect Big-Endian boolean packing");

        }
        
        [TestMethod]
        public void DiscardsExtraItemsOnFixedSize()
        {
            var original = new ConstantSizePackedBooleanClass
            {
                ConstantCountArray = GenerateBools().Take(40).ToArray(),
                ConstantLengthArray = GenerateBools().Take(30).ToArray()
            };

            var result = Roundtrip(original);

            CheckSequence(GenerateBools().Take(ConstantSizePackedBooleanClass.CountConstraint), result.ConstantCountArray);
            CheckSequence(GenerateBools().Take(ConstantSizePackedBooleanClass.LengthConstraint * 8), result.ConstantLengthArray);
        }

        [TestMethod]
        public void AddsNewItemsOnFixedSize()
        {
            var original = new ConstantSizePackedBooleanClass
            {
                ConstantCountArray = new[] { true },
                ConstantLengthArray = new[] { true },
            };

            var result = Roundtrip(original);

            bool[] expectedCount = new bool[ConstantSizePackedBooleanClass.CountConstraint];
            expectedCount[0] = true;

            bool[] expectedLength = new bool[ConstantSizePackedBooleanClass.LengthConstraint * 8];
            expectedLength[0] = true;

            CheckSequence(expectedCount, result.ConstantCountArray);
            CheckSequence(expectedLength, result.ConstantLengthArray);
        }

        [TestMethod]
        public void DoesntAffectUnpackedBooleanArrays()
        {
            var original = new UnpackedBooleanClass
            {
                UnpackedArray = new[] { true, true, false, false, true, true }
            };

            var expected = new byte[] { 6, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1 };
            var actual = Serialize(original);

            CheckSequence(expected, actual);

            var deserialized = Deserialize<UnpackedBooleanClass>(actual);

            Assert.AreEqual(original.UnpackedArray.Length, deserialized.UnpackedArrayLength, "Invalid length binding on unpacked boolean array.");
            Assert.AreEqual(original.UnpackedArray.Length, deserialized.UnpackedArrayCount, "Invalid count binding on unpacked boolean array.");

            CheckSequence(original.UnpackedArray, deserialized.UnpackedArray);
        }

        private void CheckSequence<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count(), "Incorrect length");

            var zipped = expected.Zip(actual, Tuple.Create);
            int i = 0;
            foreach (var item in zipped)
            {
                Assert.AreEqual(item.Item1, item.Item2, "Mismatch at value {0}", i);
                i++;
            }
        }

        private IEnumerable<bool> GenerateBools()
        {
            bool[] toRepeat = new bool[] { true, false, false, true, true, false, true, false, true, true, false, true };
            
            while (true)
            {
                foreach (var b in toRepeat)
                    yield return b;
            }
        }

    }
}
