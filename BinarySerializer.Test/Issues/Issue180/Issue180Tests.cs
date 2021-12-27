namespace BinarySerialization.Test.Issues.Issue180;

[TestClass]
public class Issue180Tests : TestBase
{
    public class BitFieldsAlone
    {
        [FieldOrder(0)]
        [FieldBitLength(7)]
        public byte SevenBits;
        [FieldOrder(1)]
        [FieldBitLength(1)]
        public byte OneBit;
    }

    public class BitFieldsPrecededByByte
    {
        [FieldOrder(0)]
        public byte First;

        [FieldOrder(1)]
        [FieldBitLength(7)]
        public byte SevenBits;

        [FieldOrder(2)]
        [FieldBitLength(1)]
        public byte OneBit;
    }

    [TestMethod]
    public void TestOutOfMemory()
    {
        var maxLength = 7;

        for (int i = 0; i < maxLength; i++)
        {
            Console.WriteLine($"Trying to deserialize {nameof(BitFieldsAlone)} with array size {i}");
            var serialized = new byte[i];
            var _ = Deserialize<BitFieldsAlone>(serialized);
        }

        for (int i = 0; i < maxLength; i++)
        {
            Console.WriteLine($"Trying to deserialize array of {nameof(BitFieldsAlone)} with array size {i}");
            var serialized = new byte[i];
            var _ = Deserialize<BitFieldsAlone[]>(serialized);
        }

        for (int i = 0; i < maxLength; i++)
        {
            Console.WriteLine($"Trying to deserialize {nameof(BitFieldsPrecededByByte)} with array size {i}");
            var serialized = new byte[i];
            var _ = Deserialize<BitFieldsPrecededByByte>(serialized);
        }

        for (int i = 0; i < maxLength; i += 2)
        {
            Console.WriteLine($"Trying to deserialize array of complete {nameof(BitFieldsPrecededByByte)} with array size {i}");
            var serialized = new byte[i];
            var _ = Deserialize<BitFieldsPrecededByByte[]>(serialized);
        }

        for (int i = 0; i < maxLength; i++)
        {
            Console.WriteLine($"Trying to deserialize array of partial {nameof(BitFieldsPrecededByByte)} with array size {i}");
            var serialized = new byte[i];
            // This call will never return and consumes all available memory
            var _ = Deserialize<BitFieldsPrecededByByte[]>(serialized);
        }
    }
}
