using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using BinarySerialization;
using Test_BinarySerialzer;
using BinarySerialization.Test.Misc;
using System.IO;
using BinarySerialization.Test.Issues.Issue106;

namespace BinarySerialization.Test.Issues.issue199
{
    [TestClass]
    public class Issue199Tests : TestBase
    {
        BinarySerializer serializer = new BinarySerializer();
        [TestMethod]
        public void Test()
        {
            var sourcebuffer = StringToByte("A ED7 F13 F25 F26 EBC E82 E60 E43 DD9 DC3 ");
            var testType = serializer.Deserialize<OutputChannelInt>(sourcebuffer);
            Assert.AreEqual(testType.DataAmount, testType.DistDatas.Count);
            Assert.AreEqual(Convert.ToInt32("ED7", 16), testType.DistDatas[0].Data);
            var stream = new MemoryStream();
             serializer.Serialize(stream, testType);
             var serializeBuffer = stream.ToArray();
             CollectionAssert.AreEqual(sourcebuffer, serializeBuffer);
        }

        public static byte[] StringToByte(string str)
        {
            byte[] bytes = new byte[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                bytes[i] = (byte)str[i];
            }

            return bytes;
        }

    }
}