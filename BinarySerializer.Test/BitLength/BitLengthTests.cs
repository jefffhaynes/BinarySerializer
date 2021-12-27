namespace BinarySerialization.Test.BitLength;

[TestClass]
public class BitLengthTests : TestBase
{
    [TestMethod]
    public void LengthTest()
    {
        var expected = new BitLengthClass
        {
            A = 0b1_0110_1110_1111_0111_1101,
            B = 0b111,
            C = (TypeCode)0b1101,
            Internal = new InternalBitLengthClass { Value = 0b1111 },
            Internal2 = new InternalBitLengthClass { Value = 0b10101010 }
        };

        var actual = Roundtrip(expected, new byte[] { 0x7d, 0xef, 0xf6, 0xfd, 0xaa });
        Assert.AreEqual(expected.A, actual.A);
        Assert.AreEqual(expected.B, actual.B);
        Assert.AreEqual(expected.C, actual.C);
        Assert.AreEqual(expected.Internal.Value, actual.Internal.Value);
        Assert.AreEqual(0b1010, actual.Internal2.Value);
    }

    //[TestMethod]
    public void LengthTestBE()
    {
        var expected = new BitLengthClass
        {
            A = 0b1_0110_1110_1111_0111_1101,
            B = 0b111,
            C = (TypeCode)0b1101,
            Internal = new InternalBitLengthClass { Value = 0b1111 },
            Internal2 = new InternalBitLengthClass { Value = 0b10101010 }
        };

        var actual = RoundtripBigEndian(expected, new byte[] { 0x7d, 0xef, 0xf6, 0xfd, 0xaa });
        Assert.AreEqual(expected.A, actual.A);
        Assert.AreEqual(expected.B, actual.B);
        Assert.AreEqual(expected.C, actual.C);
        Assert.AreEqual(expected.Internal.Value, actual.Internal.Value);
        Assert.AreEqual(0b1010, actual.Internal2.Value);
    }

    [TestMethod]
    public void TestBitLengthValue()
    {
        var expected = new BitLengthValueClass
        {
            Value = new InternalBitLengthValueClass
            {
                Value = 1,
                Value2 = 1
            },
            Value2 = 0x11
        };

        var actual = Roundtrip(expected);

        Assert.AreEqual(expected.Value.Value, actual.Value.Value);
        Assert.AreEqual(expected.Value.Value2, actual.Value.Value2);
        Assert.AreEqual(actual.Crc2, actual.Crc);
    }

    [TestMethod]
    public void BitLengthBoolTest()
    {
        var expected = new List<BitLengthBoolClass>
            {
                new BitLengthBoolClass {Value = true},
                new BitLengthBoolClass {Value = false},
                new BitLengthBoolClass {Value = true},
                new BitLengthBoolClass {Value = false},
                new BitLengthBoolClass {Value = true},
                new BitLengthBoolClass {Value = false},
                new BitLengthBoolClass {Value = true},
                new BitLengthBoolClass {Value = false}
            };

        var actual = Roundtrip(expected, new byte[] { 0x55 });

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.AreEqual(expected[i].Value, actual[i].Value);
        }
    }

    [TestMethod]
    public void BoundBitLengthTest()
    {
        var expected = new BoundBitLengthClass
        {
            Items = new List<InternalBitLengthClass>
                {
                    new InternalBitLengthClass {Value = 1},
                    new InternalBitLengthClass {Value = 2}
                }
        };

        var actual = Roundtrip(expected);
        Assert.AreEqual(8, actual.BitLength);
        Assert.AreEqual(expected.Items.Count, actual.Items.Count);
        Assert.AreEqual(expected.Items[0].Value, actual.Items[0].Value);
        Assert.AreEqual(expected.Items[1].Value, actual.Items[1].Value);
    }
}

