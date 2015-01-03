using System.IO;
using BinarySerializer.Test.Misc;

namespace BinarySerializerTester
{
    class Program
    {
        static void Main(string[] args)
        {
            //var test = new BinarySerializer.Test.BinarySerializerTests();

            //for (int i = 0; i < 10000; i++)
            //{
            //    test.Roundtrip();
            //}

            var ser = new BinarySerialization.BinarySerializer();
            var arr = new IntArray64K() { Array = new int[65536] };
            byte[] data;

            using (var ms = new MemoryStream())
            {
                ser.Serialize(ms, arr);
                data = ms.ToArray();
            }

            for (int i = 0; i < 1000; i++)
            {
                using (var ms = new MemoryStream(data))
                {
                    var des = ser.Deserialize<IntArray64K>(ms);
                }
            }
        }
    }
}
