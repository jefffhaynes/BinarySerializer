﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;

namespace BinarySerialization.Test.Issues.Issue25
{
    //    public class Issue25Tests
    //{
    //    [Fact]
    //   [ExpectedException(typeof(InvalidOperationException))]
    //    public void TestStackOverflow()
    //    {
    //        var expetedBytes = new byte[] { 0x00, 0x00, 0x00, 0x00 };

    //        var obj = new LoadCarrierData() { Data = new object() };

    //        var serializer = new BinarySerializer() { Endianness = BinarySerialization.Endianness.Little };

    //        byte[] actualBytes = null;

    //        var exception = Record.Exception( (using (var stream = new MemoryStream())
    //        {
    //            serializer.Serialize(stream, obj);
    //            actualBytes = stream.ToArray();
    //        }

    //        if (!expetedBytes.SequenceEqual(actualBytes))
    //        {
    //            Assert.True(true,
    //                string.Format(
    //                    CultureInfo.InvariantCulture,
    //                    "Bytes are not equal. Expected({2}): {0} Result({3}): {1}",
    //                    BitConverter.ToString(expetedBytes),
    //                    BitConverter.ToString(actualBytes),
    //                    expetedBytes.Length,
    //                    actualBytes.Length));
    //        }
            
    //    }
    //}
}
