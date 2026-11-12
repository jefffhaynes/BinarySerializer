using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Misc
{
    [TestClass]
    public class CollectionMiscTests : TestBase
    {
        [TestMethod]
        public void ListAtRootTest()
        {
            var expected = new List<string> {"1", "2", "3"};
            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Count, actual.Count);
        }

        [TestMethod]
        public void ArrayAtRootTest()
        {
            var expected = new[] {"a", "b", "c"};
            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Length, actual.Length);
        }

        [TestMethod]
        public void ListWithoutParameterlessConstructorThrowsDescriptiveError()
        {
            /* Simulates a trimmed-away collection constructor (e.g. List<TEnum> under Native AOT):
               the serializer must fail with a descriptive exception, not a NullReferenceException. */
            var expected = new NoConstructorListContainer
            {
                Field = new NoConstructorList(2) {new EmptyClass(), new EmptyClass()}
            };

            var stream = new MemoryStream();

            var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            {
                Serializer.Serialize(stream, expected);
                stream.Position = 0;
                Serializer.Deserialize<NoConstructorListContainer>(stream);
            });

            var descriptive = false;
            for (var e = (Exception) exception; e != null; e = e.InnerException)
            {
                if (e.Message.Contains("No public parameterless constructor"))
                {
                    descriptive = true;
                    break;
                }
            }

            Assert.IsTrue(descriptive,
                $"Expected a descriptive constructor error but got: {exception}");
        }
    }
}