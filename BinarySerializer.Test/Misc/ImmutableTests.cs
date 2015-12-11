﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Misc
{
    [TestClass]
    public class ImmutableTests : TestBase
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PrivateSetterTest()
        {
            var expected = new PrivateSetterClass();
            Roundtrip(expected);
        }

        [TestMethod]
        public void ImmutableTest()
        {
            var expected = new ImmutableClass(3, 4);
            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Value, actual.Value);
        }

        [TestMethod]
        public void ImmutableTest2()
        {
            var expected = new ImmutableClass2(3, 4);
            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Value, actual.Value);
        }

        [TestMethod]
        public void ImmutableWithNullableParametersTest()
        {
            var expected = new ImmutableClass3(33);
            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Value, actual.Value);
        }

        [TestMethod]
        public void ImmutableWithNullableParametersAndIgnoreTest()
        {
            var expected = new ImmutableClass4(4, 5);
            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Header, actual.Header);
            Assert.IsNull(actual.ResponseId);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ImmutableNoPublicConstructorTest()
        {
            var stream = new MemoryStream(new[] {(byte)0x1});
            Serializer.Deserialize<ImmutableNoPublicConstructorClass>(stream);
        }

        [TestMethod]
        public void ImmutableItemsList()
        {
            var expected = new List<ImmutableClass>
            {
                new ImmutableClass(1, 2),
                new ImmutableClass(3, 4)
            };

            var actual = Roundtrip(expected);

            Assert.AreEqual(expected[0].Value, actual[0].Value);
            Assert.AreEqual(expected[0].Value2, actual[0].Value2);
            Assert.AreEqual(expected[1].Value, actual[1].Value);
            Assert.AreEqual(expected[1].Value2, actual[1].Value2);
        }
    }
}
