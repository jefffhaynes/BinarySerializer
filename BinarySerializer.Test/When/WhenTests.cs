﻿namespace BinarySerialization.Test.When;

[TestClass]
public class WhenTests : TestBase
{
    [TestMethod]
    public void WhenStringTest()
    {
        var expected = new WhenStringTestClass
        {
            WhatToDo = "PickOne",
            SerializeThis = 1,
            DontSerializeThis = 2,
            SerializeThisNoMatterWhat = 3
        };

        var actual = Roundtrip(expected);

        Assert.AreEqual(expected.SerializeThis, actual.SerializeThis);
        Assert.AreNotEqual(expected.DontSerializeThis, actual.DontSerializeThis);
        Assert.AreEqual(expected.SerializeThisNoMatterWhat, actual.SerializeThisNoMatterWhat);
    }

    [TestMethod]
    public void WhenIntTest()
    {
        var expected = new WhenIntTestClass
        {
            WhatToDo = 2,
            SerializeThis = 100,
            DontSerializeThis = 200,
            SerializeThisNoMatterWhat = 300,
            SerializeThis2 = 400,
            DontSerializeThis2 = 500
        };

        var actual = Roundtrip(expected);

        Assert.AreEqual(expected.SerializeThis, actual.SerializeThis);
        Assert.AreNotEqual(expected.DontSerializeThis, actual.DontSerializeThis);
        Assert.AreEqual(expected.SerializeThisNoMatterWhat, actual.SerializeThisNoMatterWhat);
        Assert.AreEqual(expected.SerializeThis2, actual.SerializeThis2);
        Assert.AreNotEqual(expected.DontSerializeThis2, actual.DontSerializeThis2);
    }

    [TestMethod]
    public void WhenEnumTest()
    {
        var expected = new WhenEnumTestClass
        {
            WhatToDo = 1,
            SerializeThis = 1000,
            DontSerializeThis = 2000,
            SerializeThisNoMatterWhat = 3000
        };

        var actual = Roundtrip(expected);

        Assert.AreEqual(expected.SerializeThis, actual.SerializeThis);
        Assert.AreNotEqual(expected.DontSerializeThis, actual.DontSerializeThis);
        Assert.AreEqual(expected.SerializeThisNoMatterWhat, actual.SerializeThisNoMatterWhat);
    }

    [TestMethod]
    public void WhenConverterTest()
    {
        var expected = new WhenConverterTestClass
        {
            WhatToDo = 1,
            SerializeThis = 1000,
            DontSerializeThis = 2000,
            SerializeThisNoMatterWhat = 3000
        };

        var actual = Roundtrip(expected);

        Assert.AreEqual(expected.SerializeThis, actual.SerializeThis);
        Assert.AreNotEqual(expected.DontSerializeThis, actual.DontSerializeThis);
        Assert.AreEqual(expected.SerializeThisNoMatterWhat, actual.SerializeThisNoMatterWhat);
    }
}
