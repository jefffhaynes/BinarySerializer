using System;
using System.IO;
using System.Linq;
using BinarySerialization;
using BinarySerialization.Test.Misc;

namespace BinarySerializerTester
{
    public class SmallCase
    {
        private static readonly DateTime Y2K = new DateTime(2000, 1, 1);

        public double SecondsSinceY2K { get; set; }

        [Ignore]
        public DateTime UpdateTime
        {
            get { return Y2K.AddSeconds(SecondsSinceY2K); }
            set { SecondsSinceY2K = (value - Y2K).TotalSeconds; }
        }
    }

    public class Entry
    {
        [FieldOrder(0)]
        public byte Length { get; set; }

        [FieldOrder(1)]
        [FieldLength("Length")]
        public string Value { get; set; }
    }

    public class ElementClass
    {
        [FieldOrder(0)]
        [FieldLength(20)]
        public Entry Entry1 { get; set; }

        [FieldOrder(1)]
        [FieldLength(20)]
        public Entry Entry2 { get; set; }

        // etc
    }

    class Program
    {
        static void Main(string[] args)
        {
            //var test = new BinarySerialization.Test.BinarySerializerTests();

            //Enumerable.Range(0, 10000).AsParallel().ForAll(i =>
            //{
            //    test.Roundtrip();
            //    Console.WriteLine(i);
            //});

            //for (int i = 0; i < 10000; i++)
            //{
            //    test.Roundtrip();
            //}

            var ser = new BinarySerialization.BinarySerializer();
            var arr = new ElementClass();
            byte[] data;

            using (var ms = new MemoryStream())
            {
                ser.Serialize(ms, arr);
                data = ms.ToArray();
            }

            //for (int i = 0; i < 1000; i++)
            //{
            //    using (var ms = new MemoryStream(data))
            //    {
            //        var des = ser.Deserialize<IntArray64K>(ms);
            //    }
            //}
        }
    }
}
