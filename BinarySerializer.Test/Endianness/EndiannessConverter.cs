using System;

namespace BinarySerialization.Test.Endianness
{
    public class EndiannessConverter : IValueConverter
    {
        public const uint LittleEndiannessMagic = 0x1A2B3C4D;
        public const uint BigEndiannessMagic = 0x4D3C2B1A;

        public object Convert(object value, object parameter, BinarySerializationContext context)
        {
            var indicator = System.Convert.ToUInt32(value);
            if (indicator == LittleEndiannessMagic)
                return BinarySerialization.Endianness.Little;
            else if (indicator == BigEndiannessMagic)
                return BinarySerialization.Endianness.Big;

            throw new InvalidOperationException("Invalid endian magic");
        }

        public object ConvertBack(object value, object parameter, BinarySerializationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
