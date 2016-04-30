namespace BinarySerialization
{
    public sealed class FieldCrc16Attribute : FieldValueAttributeBase
    {
        private const ushort DefaultPolynomial = 0x1021;
        private const ushort DefaultInitialValue = 0xffff;
        
        private readonly Crc16 _crc;

        public FieldCrc16Attribute(string valuePath) : base(valuePath)
        {
            _crc = new Crc16(Polynomial, InitialValue)
            {
                IsDataReflected = IsDataReflected,
                IsRemainderReflected = IsRemainderReflected,
                FinalXor = FinalXor
            };
        }

        public ushort Polynomial { get; set; } = DefaultPolynomial;

        public ushort InitialValue { get; set; } = DefaultInitialValue;
        
        public bool IsDataReflected { get; set; } = false;

        public bool IsRemainderReflected { get; set; } = false;

        public ushort FinalXor { get; set; } = 0;

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