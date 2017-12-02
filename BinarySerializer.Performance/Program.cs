using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

//using System.Runtime.Serialization.Formatters.Binary;

namespace BinarySerializer.Performance
{
    class Program
    {
        private static void Main(string[] args)
        {
            var beer = new Beer
            {
                Alcohol = 6,

                Brand = "Brand",
                Sort = new List<SortContainer>
                {
                    new SortContainer{Name = "some sort of beer"},
                    new SortContainer {Name = "another beer"}
                },
                Brewery = "Brasserie Grain d'Orge"
            };

            DoBS(beer, 100000);
            var task = DoBSAsync(beer, 100000);
            task.Wait();
            DoBSParallel(beer, 100000);
            //DoBF(beer, 100000);
            Console.ReadKey();
        }

        private static void DoBS<T>(T obj, int iterations)
        {
            var stopwatch = new Stopwatch();

            var ser = new BinarySerialization.BinarySerializer();

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
                    ser.Deserialize<T>(ms);
                    ms.Position = 0;
                }
                stopwatch.Stop();
                Console.WriteLine("BS DESER: {0}", stopwatch.Elapsed);
                stopwatch.Reset();
            }
        }

        private static async Task DoBSAsync<T>(T obj, int iterations)
        {
            var stopwatch = new Stopwatch();

            var ser = new BinarySerialization.BinarySerializer();

            using (var ms = new MemoryStream())
            {
                stopwatch.Start();
                for (int i = 0; i < iterations; i++)
                {
                    ser.Serialize(ms, obj);
                }
                stopwatch.Stop();
                Console.WriteLine("BSA SER: {0}", stopwatch.Elapsed);
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
                    await ser.DeserializeAsync<T>(ms);
                    ms.Position = 0;
                }
                stopwatch.Stop();
                Console.WriteLine("BSA DESER: {0}", stopwatch.Elapsed);
                stopwatch.Reset();
            }
        }

        private static void DoBSParallel<T>(T obj, int iterations)
        {
            var stopwatch = new Stopwatch();

            var ser = new BinarySerialization.BinarySerializer();

            stopwatch.Start();
            var data = Enumerable.Range(0, iterations).AsParallel().Select(i =>
            {
                using (var ms = new MemoryStream())
                {
                    ser.Serialize(ms, obj);
                    return ms.ToArray();
                }
            }).ToArray();
            
            stopwatch.Stop();
            Console.WriteLine("BS || SER: {0}", stopwatch.Elapsed);
            stopwatch.Reset();
            
            stopwatch.Start();
            data.AsParallel().ForAll(d => ser.Deserialize<T>(d));
            stopwatch.Stop();
            Console.WriteLine("BS || DESER: {0}", stopwatch.Elapsed);
            stopwatch.Reset();
        }

        // BinaryFormatter will be available again in later versions of .NET Core
        //private static void DoBF(object obj, int iterations)
        //{
        //    var formatter = new BinaryFormatter();

        //    var stopwatch = new Stopwatch();

        //    using (var ms = new MemoryStream())
        //    {
        //        stopwatch.Start();
        //        for (int i = 0; i < iterations; i++)
        //        {
        //            formatter.Serialize(ms, obj);
        //        }
        //        stopwatch.Stop();
        //        Console.WriteLine("BF SER: {0}", stopwatch.Elapsed);
        //        stopwatch.GetInitial();
        //    }

        //    var dataStream = new MemoryStream();
        //    formatter.Serialize(dataStream, obj);
        //    byte[] data = dataStream.ToArray();

        //    using (var ms = new MemoryStream(data))
        //    {
        //        stopwatch.Start();
        //        for (int i = 0; i < iterations; i++)
        //        {
        //            formatter.Deserialize(ms);
        //            ms.Position = 0;
        //        }
        //        stopwatch.Stop();
        //        Console.WriteLine("BF DESER: {0}", stopwatch.Elapsed);
        //        stopwatch.GetInitial();
        //    }
        //}
    }
}