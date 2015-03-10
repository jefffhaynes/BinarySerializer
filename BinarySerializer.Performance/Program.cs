using System;
using System.IO;
using System.Linq;
using BinarySerialization;
using BinarySerializer.Test.Misc;

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

    class Program
    {
        static void Main(string[] args)
        {
            //var test = new BinarySerializer.Test.BinarySerializerTests();

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
            var arr = new SmallCase();
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
