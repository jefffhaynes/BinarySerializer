namespace BinarySerialization
{
    public sealed class FieldCrc32Attribute : FieldValueAttributeBase
    {
        private const uint DefaultPolynomial = 0xedb88320;
        private const uint DefaultInitialValue = 0xffffffff;
        private const uint DefaultFinalXor = 0xffffffff;
        
        private readonly Crc32 _crc;

        public FieldCrc32Attribute(string valuePath) : base(valuePath)
        {
            _crc = new Crc32(Polynomial, InitialValue)
            {
                IsDataReflected = IsDataReflected,
                IsRemainderReflected = IsRemainderReflected,
                FinalXor = FinalXor
            };
        }

        public uint Polynomial { get; set; } = DefaultPolynomial;

        public uint InitialValue { get; set; } = DefaultInitialValue;
        
        public bool IsDataReflected { get; set; } = true;

        public bool IsRemainderReflected { get; set; } = true;

        public uint FinalXor { get; set; } = DefaultFinalXor;

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