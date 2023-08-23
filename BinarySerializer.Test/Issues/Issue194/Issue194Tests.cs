using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue194
{
    [TestClass]
    public class Issue194Tests : TestBase
    {
        public enum Indicator
        {
            A, B, C
        }

        public class BaseClass
        {
        }

        public class ClassA : BaseClass
        {
            public int Value { get; set; }
        }

        public class ClassB : BaseClass
        {
            public short Value { get; set; }
        }

        public class ClassC : BaseClass
        {
            public byte Value { get; set; }
        }

        public class ContainerClass
        {
            [FieldOrder(0)]
            public Indicator Indicator { get; set; }

            [FieldOrder(1)]
            [Subtype(nameof(Indicator), Indicator.A, typeof(ClassA))]
            [Subtype(nameof(Indicator), Indicator.B, typeof(ClassB))]
            [Subtype(nameof(Indicator), Indicator.C, typeof(ClassC))]
            public BaseClass Value { get; set; }
        }

        [TestMethod]
        public async Task TestAsync()
        {
            var container = new ContainerClass { Value = new ClassB() };var lists = Enumerable.Repeat(container, 100);
            var tasks = lists.Select(ConvertBinaryObjectToByteArrayAsync);
            
            await Task.WhenAll(tasks);
        }

        static async Task<byte[]> ConvertBinaryObjectToByteArrayAsync(object obj)
        {
            var binarySerializer = new BinarySerializer();

            using var ms = new MemoryStream();
            await binarySerializer.SerializeAsync(ms, obj);
            return ms.ToArray();
        }
    }
}
