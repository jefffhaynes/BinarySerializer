using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test
{
    [TestClass]
    public class BinarySerializerTests
    {
        private const string Disclaimer = "This isn't really cereal";
        private BinarySerialization.BinarySerializer _serializer = new BinarySerialization.BinarySerializer();

        private static Cereal Cerealize()
        {
            var disclaimerStream = new MemoryStream();
            var writer = new StreamWriter(disclaimerStream);
            writer.Write(Disclaimer);
            writer.Flush();
            disclaimerStream.Position = 0;

            return new Cereal
                {
                    IsLittleEndian = "false",
                    Name = "Cheerios",
                    Manufacturer = "General Mills",
                    NutritionalInformation = new NutritionalInformation
                        {
                            Calories = 100,
                            Fat = 1.5f,
                            Cholesterol = 64,
                            VitaminA = 2,
                            VitaminB = 3,
                            OtherNestedStuff = new List<string> {"it's", "got", "electrolytes"},
                            OtherNestedStuff2 = new List<string> {"stuff", "plants", "crave"},
                            Toys = new List<Toy>
                                {
                                    new Toy("Truck"),
                                    new Toy("Godzilla"),
                                    new Toy("EZ Bake Oven"),
                                    new Toy("Bike", true)
                                },
                            WeirdOutlierLengthedField = "hihihihihi",
                            Ingredients = new Ingredients
                                {
                                    MainIngredient = new Iron()
                                }
                        },
                    DoubleField = 33.33333333,
                    OtherStuff = new List<string> {"apple", "pear", "banana"},
                    Shape = CerealShape.Circular,
                    DefinitelyNotTheShape = CerealShape.Square,
                    DontSerializeMe = "bro",
                    SerializeMe = "!",
                    Outlier = 4,
                    ExplicitlyTerminatedList = new List<string> {"red", "white", "blue"},
                    ImplicitlyTerminatedList =
                        new List<CerealShape> {CerealShape.Circular, CerealShape.Circular, CerealShape.Square},
                    ArrayOfInts = new[] {1, 2, 3},
                    InvalidFieldLength = "oops",
                    DisclaimerLength = disclaimerStream.Length,
                    Disclaimer = disclaimerStream
                };
        }

        [TestMethod]
        public void Roundtrip()
        {
            Cereal cereal = Cerealize();

            _serializer = new BinarySerialization.BinarySerializer();
            _serializer.MemberDeserialized += SerializerMemberDeserialized;
            _serializer.MemberSerialized += SerializerMemberSerialized;

            using (var stream = new MemoryStream())
            {
                _serializer.Serialize(stream, cereal);
                stream.Position = 0;

                Assert.AreEqual(Endianness.Big, _serializer.Endianness);

                File.WriteAllBytes("c:\\temp\\out.bin", stream.ToArray());


                var cereal2 = _serializer.Deserialize<Cereal>(stream);

                Assert.AreEqual("Cheeri", cereal2.Name);
                Assert.AreEqual(cereal.Manufacturer, cereal2.Manufacturer);
                Assert.AreEqual(cereal.NutritionalInformation.Fat, cereal2.NutritionalInformation.Fat);
                Assert.AreEqual(cereal.NutritionalInformation.Calories, cereal2.NutritionalInformation.Calories);
                Assert.AreEqual(cereal.NutritionalInformation.VitaminA, cereal2.NutritionalInformation.VitaminA);
                Assert.AreEqual(cereal.NutritionalInformation.VitaminB, cereal2.NutritionalInformation.VitaminB);
                Assert.IsTrue(cereal.NutritionalInformation.OtherNestedStuff.SequenceEqual(
                    cereal2.NutritionalInformation.OtherNestedStuff));
                Assert.IsTrue(cereal.NutritionalInformation.OtherNestedStuff2.SequenceEqual(
                    cereal2.NutritionalInformation.OtherNestedStuff2));

                Assert.IsTrue(cereal.NutritionalInformation.Toys.SequenceEqual(cereal2.NutritionalInformation.Toys));

                Assert.IsInstanceOfType(cereal.NutritionalInformation.Ingredients.MainIngredient, typeof (Iron));

                Assert.AreEqual(cereal2.DoubleField, cereal2.DoubleField);
                Assert.IsTrue(cereal2.OtherStuff.Contains("app"));
                Assert.IsTrue(cereal2.OtherStuff.Contains("pea"));
                Assert.IsTrue(cereal2.OtherStuff.Contains("ban"));
                Assert.AreEqual(3, cereal2.OtherStuff.Count);
                Assert.AreEqual(cereal2.OtherStuff.Count, cereal2.OtherStuffCount);
                Assert.AreEqual(CerealShape.Circular, cereal2.Shape);
                Assert.AreEqual(CerealShape.Square, cereal2.DefinitelyNotTheShape);
                Assert.IsNull(cereal2.DontSerializeMe);
                Assert.AreEqual(cereal.SerializeMe, cereal2.SerializeMe);
                Assert.AreEqual(3, cereal2.ArrayOfInts.Length);
                Assert.AreEqual(1, cereal2.ArrayOfInts[0]);
                Assert.AreEqual(2, cereal2.ArrayOfInts[1]);
                Assert.AreEqual(3, cereal2.ArrayOfInts[2]);
                Assert.AreEqual(cereal.Outlier, cereal2.Outlier);

                Assert.IsTrue(cereal.ExplicitlyTerminatedList.SequenceEqual(cereal2.ExplicitlyTerminatedList));
                Assert.IsTrue(cereal.ImplicitlyTerminatedList.SequenceEqual(cereal2.ImplicitlyTerminatedList));

                string weirdOutlierLengthedField = cereal.NutritionalInformation.WeirdOutlierLengthedField;
                Assert.AreEqual(weirdOutlierLengthedField.Substring(0, (int)cereal.Outlier*2),
                                cereal2.NutritionalInformation.WeirdOutlierLengthedField);

                var reader = new StreamReader(cereal2.Disclaimer);
                Assert.AreEqual(Disclaimer, reader.ReadToEnd());
            }
        }


        private void SerializerMemberSerialized(object sender, MemberSerializedEventArgs e)
        {
            if (e.MemberName == "IsLittleEndian")
            {
                bool isLittleEndian = bool.Parse((string) e.Value);
                if (!isLittleEndian)
                    _serializer.Endianness = Endianness.Big;
            }

            Console.WriteLine("write {0}: {1}", e.MemberName, e.Value);
        }

        private void SerializerMemberDeserialized(object sender, MemberSerializedEventArgs e)
        {
            if (e.MemberName == "IsLittleEndian")
            {
                bool isLittleEndian = bool.Parse((string) e.Value);
                if (!isLittleEndian)
                    _serializer.Endianness = Endianness.Big;
            }

            Console.WriteLine("read {0}: {1}", e.MemberName, e.Value);
        }

        [TestMethod]
        public void NonSeekableStreamSerializationTest()
        {
            var stream = new NonSeekableStream();
            var serializer = new BinarySerialization.BinarySerializer();
            serializer.Serialize(stream, new Iron());
        }

        [TestMethod]
        public void NonSeekableStreamDeserializationTest()
        {
            var stream = new NonSeekableStream();
            var serializer = new BinarySerialization.BinarySerializer();
            serializer.Serialize(stream, new Iron());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NonSeekableStreamWithOffsetAttributeShouldThrowInvalidOperationException()
        {
            var stream = new NonSeekableStream();
            var serializer = new BinarySerialization.BinarySerializer();
            serializer.Serialize(stream, Cerealize());
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void NullStreamSerializationShouldThrowNullArgumentException()
        {
            var serializer = new BinarySerialization.BinarySerializer();
            serializer.Serialize(null, new object());
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void NullGraphSerializationShouldThrowNullArgumentException()
        {
            var serializer = new BinarySerialization.BinarySerializer();
            serializer.Serialize(new MemoryStream(), null);
        }

        [TestMethod]
#if DEBUG
        [ExpectedException(typeof(ArgumentNullException))]
#else
        [ExpectedException(typeof(InvalidOperationException))]
#endif
        public void NullMemberSerializationShouldThrowException()
        {
            var serializer = new BinarySerialization.BinarySerializer();
            serializer.Serialize(new MemoryStream(), new NullArrayClass());
        }

        [TestMethod]
        public void UnresolvedSubtypeMemberDeserializationYieldsNull()
        {
            var serializer = new BinarySerialization.BinarySerializer();
            var ingredients = serializer.Deserialize<Ingredients>(new byte[] {0x4});
            Assert.AreEqual(null, ingredients.MainIngredient);
        }

        [TestMethod]
        public void ImplicitTermination()
        {
            var data = new byte[] {0x0, 0x1, 0x2, 0x3};

            var serializer = new BinarySerialization.BinarySerializer();
            var byteList = serializer.Deserialize<ImplictTermination>(data);

            Assert.AreEqual(4, byteList.Data.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void CollectionAtRootShouldThrowNotSupportedException()
        {
            var serializer = new BinarySerialization.BinarySerializer();
            serializer.Deserialize<List<string>>(new byte[3]);
        }
    }
}