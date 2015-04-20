using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BinarySerialization.Performance
{
    class Program
    {
        private static void Main(string[] args)
        {
            DoBS(100000);
            DoBF(100000);
            Console.ReadKey();
        }

        private static void DoBS(int iterations)
        {
            var stopwatch = new Stopwatch();

            var ser = new BinarySerializer();
            var obj = new BasicClass();

            using (var ms = new MemoryStream())
            {
                stopwatch.Start();
                for (int i = 0; i < iterations; i++)
                {
                    ser.Serialize(ms, obj);
                }
                stopwatch.Stop();
                Console.WriteLine("BS SER: {0}", stopwatch.Elapsed);
                stopwatch.Reset();
            }

            var dataStream = new MemoryStream();
            ser.Serialize(dataStream, obj);
            byte[] data = dataStream.ToArray();

            using (var ms = new MemoryStream(data))
            {
                stopwatch.Start();
                for (int i = 0; i < iterations; i++)
                {
                    ser.Deserialize<BasicClass>(ms);
                    ms.Position = 0;
                }
                stopwatch.Stop();
                Console.WriteLine("BS DESER: {0}", stopwatch.Elapsed);
                stopwatch.Reset();
            }
        }

        private static void DoBF(int iterations)
        {
            var formatter = new BinaryFormatter();

            var stopwatch = new Stopwatch();

            var obj = new BasicClass();

            using (var ms = new MemoryStream())
            {
                stopwatch.Start();
                for (int i = 0; i < iterations; i++)
                {
                    formatter.Serialize(ms, obj);
                }
                stopwatch.Stop();
                Console.WriteLine("BF SER: {0}", stopwatch.Elapsed);
                stopwatch.Reset();
            }

            var dataStream = new MemoryStream();
            formatter.Serialize(dataStream, obj);
            byte[] data = dataStream.ToArray();

            using (var ms = new MemoryStream(data))
            {
                stopwatch.Start();
                for (int i = 0; i < iterations; i++)
                {
                    formatter.Deserialize(ms);
                    ms.Position = 0;
                }
                stopwatch.Stop();
                Console.WriteLine("BF DESER: {0}", stopwatch.Elapsed);
                stopwatch.Reset();
            }
        }
    }
}
