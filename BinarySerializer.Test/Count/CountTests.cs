namespace BinarySerialization.Test.Count;

[TestClass]
public class CountTests : TestBase
{
    [TestMethod]
    public void ConstCountTest()
    {
        var actual = Roundtrip(new ConstCountClass<string>
        {
            Field = new List<string>(TestSequence),
            Field2 = TestSequence.ToArray()
        });

        Assert.AreEqual(3, actual.Field.Count);
        Assert.AreEqual(3, actual.Field2.Length);
    }

    [TestMethod]
    public void PrimitiveConstCountTest()
    {
        var actual = Roundtrip(new ConstCountClass<int>
        {
            Field = new List<int>(PrimitiveTestSequence),
            Field2 = PrimitiveTestSequence.ToArray()
        });

        Assert.AreEqual(3, actual.Field.Count);
        Assert.AreEqual(3, actual.Field2.Length);
    }

    [TestMethod]
    public void CountTest()
    {
        var expected = new BoundCountClass
        {
            Field = new List<string>(TestSequence)
        };

        var actual = Roundtrip(expected);
        Assert.AreEqual(TestSequence.Length, actual.Field.Count);
        Assert.AreEqual(TestSequence.Length, actual.FieldCountField);
        Assert.IsTrue(expected.Field.SequenceEqual(actual.Field));
    }

    [TestMethod]
    public void ConstCountMismatchTest()
    {
        var actual = Roundtrip(new ConstCountClass<string> { Field = new List<string>(TestSequence.Take(2)) });
        Assert.AreEqual(3, actual.Field.Count);
    }

    [TestMethod]
    public void PrimitiveConstCountMismatchTest()
    {
        var actual = Roundtrip(new ConstCountClass<int>
        {
            Field = new List<int>(PrimitiveTestSequence.Take(2)),
            Field2 = PrimitiveTestSequence.Take(2).ToArray()
        });
        Assert.AreEqual(3, actual.Field.Count);
    }

    [TestMethod]
    public void PrimitiveListBindingTest()
    {
        var expected = new PrimitiveListBindingClass { Ints = new List<int> { 1, 2, 3 } };
        var actual = Roundtrip(expected);

        Assert.AreEqual(expected.Ints.Count, actual.ItemCount);
    }

    [TestMethod]
    public void PrimitiveArrayBindingTest()
    {
        var expected = new PrimitiveArrayBindingClass { Ints = new[] { 1, 2, 3 } };
        var actual = Roundtrip(expected);

        Assert.AreEqual(expected.Ints.Length, actual.ItemCount);
    }

    [TestMethod]
    public void EmptyListBindingTest()
    {
        var expected = new PrimitiveListBindingClass();
        var actual = Roundtrip(expected);

        Assert.AreEqual(0, actual.Ints.Count);
    }

    [TestMethod]
    public void EmptyArrayBindingTest()
    {
        var expected = new PrimitiveArrayBindingClass();
        var actual = Roundtrip(expected);

        Assert.AreEqual(0, actual.Ints.Length);
    }

    [TestMethod]
    public void MultibindingTest()
    {
        var expected = new MultibindingClass
        {
            Items = new List<string>(new[] { "hello", "world" })
        };

        var actual = Roundtrip(expected);

        Assert.AreEqual(2, actual.Count);
        Assert.AreEqual(2, actual.Count2);
    }

    [TestMethod]
    public void PaddedConstSizeListTest()
    {
        var expected = new PaddedConstSizedListClass
        {
            Items = new List<PaddedConstSizeListItemClass>()
        };

        var actual = Roundtrip(expected);
        Assert.AreEqual(6, actual.Items.Count);
    }

    [TestMethod]
    public void PrimitiveNullByteArrayTest()
    {
        PrimitiveNullArrayLengthTest<byte>(sizeof(byte));
    }

    [TestMethod]
    public void PrimitiveNullSByteArrayTest()
    {
        PrimitiveNullArrayLengthTest<sbyte>(sizeof(sbyte));
    }

    [TestMethod]
    public void PrimitiveNullShortArrayTest()
    {
        PrimitiveNullArrayLengthTest<short>(sizeof(short));
    }

    [TestMethod]
    public void PrimitiveNullUShortArrayTest()
    {
        PrimitiveNullArrayLengthTest<ushort>(sizeof(ushort));
    }

    [TestMethod]
    public void PrimitiveNullIntArrayTest()
    {
        PrimitiveNullArrayLengthTest<int>(sizeof(int));
    }

    [TestMethod]
    public void PrimitiveNullUIntArrayTest()
    {
        PrimitiveNullArrayLengthTest<uint>(sizeof(uint));
    }

    [TestMethod]
    public void PrimitiveNullLongArrayTest()
    {
        PrimitiveNullArrayLengthTest<uint>(sizeof(uint));
    }

    [TestMethod]
    public void PrimitiveNullULongArrayTest()
    {
        PrimitiveNullArrayLengthTest<uint>(sizeof(uint));
    }

    [TestMethod]
    public void PrimitiveNullStringArrayTest()
    {
        PrimitiveNullArrayLengthTest<string>(0);
    }

    private void PrimitiveNullArrayLengthTest<TValue>(int expectedChildLength)
    {
        var expected = new PrimitiveArrayConstClass<TValue>();
        Roundtrip(expected, expectedChildLength * 5);
    }
}
