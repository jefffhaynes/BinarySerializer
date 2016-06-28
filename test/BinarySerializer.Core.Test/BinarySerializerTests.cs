using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace BinarySerialization.Test
{
        public class BinarySerializerTests
    {
        private const string Disclaimer = "This isn't really cereal";
        private readonly BinarySerializer _serializer = new BinarySerializer();

        public BinarySerializerTests()
        {
            _serializer.MemberDeserialized += SerializerMemberDeserialized;
            _serializer.MemberSerialized += SerializerMemberSerialized;
        }

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
                Outlier = 0,
                ExplicitlyTerminatedList = new List<string> {"red", "white", "blue"},
                ImplicitlyTerminatedList =
                    new List<CerealShape> {CerealShape.Circular, CerealShape.Circular, CerealShape.Square},
                ArrayOfInts = new[] {1, 2, 3},
                InvalidFieldLength = "oops",
                DisclaimerLength = disclaimerStream.Length,
                Disclaimer = disclaimerStream
            };
        }

        [Fact]
        public void Roundtrip()
        {
            Cereal cereal = Cerealize();


            using (var stream = new MemoryStream())
            {
                _serializer.Serialize(stream, cereal);
                stream.Position = 0;

                Assert.Equal(BinarySerialization.Endianness.Big, _serializer.Endianness);

                //File.WriteAllBytes("c:\\temp\\out.bin", stream.ToArray());


                var cereal2 = _serializer.Deserialize<Cereal>(stream);

                Assert.Equal("Cheeri", cereal2.Name);
                Assert.Equal(cereal.Manufacturer, cereal2.Manufacturer);
                Assert.Equal(cereal.NutritionalInformation.Fat, cereal2.NutritionalInformation.Fat);
                Assert.Equal(cereal.NutritionalInformation.Calories, cereal2.NutritionalInformation.Calories);
                Assert.Equal(cereal.NutritionalInformation.VitaminA, cereal2.NutritionalInformation.VitaminA);
                Assert.Equal(cereal.NutritionalInformation.VitaminB, cereal2.NutritionalInformation.VitaminB);
                Assert.True(cereal.NutritionalInformation.OtherNestedStuff.SequenceEqual(
                    cereal2.NutritionalInformation.OtherNestedStuff));
                Assert.True(cereal.NutritionalInformation.OtherNestedStuff2.SequenceEqual(
                    cereal2.NutritionalInformation.OtherNestedStuff2));

                Assert.True(cereal.NutritionalInformation.Toys.SequenceEqual(cereal2.NutritionalInformation.Toys));

                Assert.IsAssignableFrom(typeof (Iron),cereal.NutritionalInformation.Ingredients.MainIngredient);

                Assert.Equal(cereal2.DoubleField, cereal2.DoubleField);
                Assert.True(cereal2.OtherStuff.Contains("app"));
                Assert.True(cereal2.OtherStuff.Contains("pea"));
                Assert.True(cereal2.OtherStuff.Contains("ban"));
                Assert.Equal(3, cereal2.OtherStuff.Count);
                Assert.Equal(cereal2.OtherStuff.Count, cereal2.OtherStuffCount);
                Assert.Equal(CerealShape.Circular, cereal2.Shape);
                Assert.Equal(CerealShape.Square, cereal2.DefinitelyNotTheShape);
                Assert.Null(cereal2.DontSerializeMe);
                Assert.Equal(cereal.SerializeMe, cereal2.SerializeMe);
                Assert.Equal(3, cereal2.ArrayOfInts.Length);
                Assert.Equal(1, cereal2.ArrayOfInts[0]);
                Assert.Equal(2, cereal2.ArrayOfInts[1]);
                Assert.Equal(3, cereal2.ArrayOfInts[2]);
                Assert.Equal(cereal.NutritionalInformation.WeirdOutlierLengthedField.Length/2.0, cereal2.Outlier);

                Assert.True(cereal.ExplicitlyTerminatedList.SequenceEqual(cereal2.ExplicitlyTerminatedList));
                Assert.True(cereal.ImplicitlyTerminatedList.SequenceEqual(cereal2.ImplicitlyTerminatedList));

                //string weirdOutlierLengthedField = cereal.NutritionalInformation.WeirdOutlierLengthedField;
                //Assert.AreEqual(weirdOutlierLengthedField.Substring(0, (int)cereal.Outlier*2),
                //                cereal2.NutritionalInformation.WeirdOutlierLengthedField);

                var reader = new StreamReader(cereal2.Disclaimer);
                Assert.Equal(Disclaimer, reader.ReadToEnd());
            }
        }


        private void SerializerMemberSerialized(object sender, MemberSerializedEventArgs e)
        {
            if (e.MemberName == "IsLittleEndian")
            {
                bool isLittleEndian = bool.Parse((string) e.Value);
                if (!isLittleEndian)
                    _serializer.Endianness = BinarySerialization.Endianness.Big;
            }

            Console.WriteLine("write {0}: {1} @ {2}", e.MemberName, e.Value, e.Offset);
        }

        private void SerializerMemberDeserialized(object sender, MemberSerializedEventArgs e)
        {
            if (e.MemberName == "IsLittleEndian")
            {
                bool isLittleEndian = bool.Parse((string) e.Value);
                if (!isLittleEndian)
                    _serializer.Endianness = BinarySerialization.Endianness.Big;
            }

            Console.WriteLine("read {0}: {1} @ {2}", e.MemberName, e.Value, e.Offset);
        }

        [Fact]
        public void NonSeekableStreamSerializationTest()
        {
            var stream = new NonSeekableStream();
            var serializer = new BinarySerializer();
            serializer.Serialize(stream, new Iron());
        }

        [Fact]
        public void NonSeekableStreamDeserializationTest()
        {
            var stream = new NonSeekableStream();
            var serializer = new BinarySerializer();
            serializer.Serialize(stream, new Iron());
        }

        [Fact]
        //[ExpectedException(typeof (InvalidOperationException))]
        public void NonSeekableStreamWithOffsetAttributeShouldThrowInvalidOperationException()
        {
            var stream = new NonSeekableStream();
            var serializer = new BinarySerializer();
            var e = Record.Exception(() => 
            serializer.Serialize(stream, Cerealize()));
            Assert.NotNull(e);
            Assert.IsType<InvalidOperationException>(e);
        }

        [Fact]
        //[ExpectedException(typeof (ArgumentNullException))]
        public void NullStreamSerializationShouldThrowNullArgumentException()
        {
            var serializer = new BinarySerializer();
            var e = Record.Exception(() =>
            serializer.Serialize(null, new object()));
            Assert.NotNull(e);
            Assert.IsType<ArgumentNullException>(e);
        }

        [Fact]
        public void NullGraphSerializationShouldSerializeNothing()
        {
            var serializer = new BinarySerializer();
            var stream = new MemoryStream();
            serializer.Serialize(stream, null);
            Assert.Equal(0, stream.Length);
        }

//        [TestMethod]
//#if DEBUG
//        [ExpectedException(typeof(ArgumentNullException))]
//#else
//        [ExpectedException(typeof(InvalidOperationException))]
//#endif
//        public void NullMemberSerializationShouldThrowException()
//        {
//            var serializer = new BinarySerialization.BinarySerializer();
//            serializer.Serialize(new MemoryStream(), new NullArrayClass());
//        }

        [Fact]
        public void UnresolvedSubtypeMemberDeserializationYieldsNull()
        {
            var serializer = new BinarySerializer();
            var ingredients = serializer.Deserialize<Ingredients>(new byte[] {0x4});
            Assert.Equal(null, ingredients.MainIngredient);
        }

        [Fact]
        public void ImplicitTermination()
        {
            var data = new byte[] {0x0, 0x1, 0x2, 0x3};

            var serializer = new BinarySerializer();
            var byteList = serializer.Deserialize<ImplictTermination>(data);

            Assert.Equal(4, byteList.Data.Count);
        }

        //[TestMethod]
        //[ExpectedException(typeof(NotSupportedException))]
        //public void CollectionAtRootShouldThrowNotSupportedException()
        //{
        //    var serializer = new BinarySerialization.BinarySerializer();
        //    serializer.Deserialize<List<string>>(new byte[3]);
        //}
    }
}