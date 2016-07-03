namespace BinarySerialization
{
    internal sealed class Crc16 : Crc<ushort>
    {
        public Crc16(ushort polynomial, ushort initialValue) : base(polynomial, initialValue)
        {
        }

        protected override int Width => 16;

        protected override uint ToUInt32(ushort value)
        {
            return value;
        }

        protected override ushort FromUInt32(uint value)
        {
            return (ushort) value;
        }
    }
}