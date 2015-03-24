using System;
using System.IO;
using System.Linq;
using BinarySerialization;
using BinarySerialization.Test.Issues.Issue9;
using BinarySerialization.Test.Misc;

namespace BinarySerializerTester
{
   

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

            var ser = new BinarySerializer();
            var arr = new ElementClass();
            byte[] data;

            using (var ms = new MemoryStream())
            {
                ser.Serialize(ms, arr);
                data = ms.ToArray();
            }

            for (int i = 0; i < 100000; i++)
            {
                using (var ms = new MemoryStream(data))
                {
                    var des = ser.Deserialize<ElementClass>(ms);
                }
            }
        }
    }
}
