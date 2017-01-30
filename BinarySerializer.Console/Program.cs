using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BinarySerializer.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var beer = new Beer
            {
                Alcohol = 6,

                Brand = "Brand",
                Sort = new List<SortContainer>
                {
                    new SortContainer{Name = "some sort of beer"}
                },
                Brewery = "Brasserie Grain d'Orge"
            };

            DoBS(beer, 100000);
        }

        private static void DoBS<T>(T obj, int iterations)
        {
            var ser = new BinarySerialization.BinarySerializer();

            var stopwatch = new Stopwatch();
            using (var ms = new MemoryStream())
            {
                stopwatch.Start();
                for (int i = 0; i < iterations; i++)
                {
                    ser.Serialize(ms, obj);
                }
                stopwatch.Stop();
                System.Console.WriteLine("BS SER: {0}", stopwatch.Elapsed);
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
                System.Console.WriteLine("BS DESER: {0}", stopwatch.Elapsed);
                stopwatch.Reset();
            }
        }
    }
}
