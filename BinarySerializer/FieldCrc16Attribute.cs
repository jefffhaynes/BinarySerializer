namespace BinarySerialization
{
    public class FieldCrc16Attribute : FieldValueAttribute
    {
        private const ushort DefaultPolynomial = 0x1021;
        private const ushort DefaultInitialValue = 0xffff;

        private readonly ushort[] _table;
        private Crc16 _crc;

        public FieldCrc16Attribute(string valuePath) : base(valuePath)
        {
            _crc = new Crc16(Polynomial, InitialValue);
        }

        public ushort Polynomial { get; set; } = DefaultPolynomial;

        public ushort InitialValue { get; set; } = DefaultInitialValue;

        protected override void Reset(object fieldValue)
        {
            _crc.Reset();
        }

        protected override void Compute(byte[] buffer, int offset, int count)
        {
            _crc.Compute(buffer, offset, count);
        }

        protected override object ComputeFinal()
        {
            return _crc.ComputeFinal();
        }
    }
}