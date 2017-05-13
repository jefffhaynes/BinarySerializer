using System;

namespace BinarySerialization
{
    /// <summary>
    ///     Specifies an 8-bit checksum for a member or object subgraph, defined as the 2's compliment of the addition of all
    ///     bytes.
    /// </summary>
    public class FieldChecksumAttribute : FieldValueAttributeBase
    {
        private byte _checksum;

        /// <summary>
        ///     Initializes a new instance of the FieldChecksum class.
        /// </summary>
        public FieldChecksumAttribute(string checksumPath) : base(checksumPath)
        {
        }

        /// <summary>
        ///     The mode used to compute the checksum.
        /// </summary>
        public ChecksumMode Mode { get; set; }

        /// <summary>
        ///     This is called by the framework to indicate a new operation.
        /// </summary>
        /// <param name="context"></param>
        protected override void Reset(BinarySerializationContext context)
        {
            _checksum = 0;
        }

        /// <summary>
        ///     This is called one or more times by the framework to add data to the computation.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        protected override void Compute(byte[] buffer, int offset, int count)
        {
            if (Mode == ChecksumMode.Xor)
            {
                for (var i = offset; i < count; i++)
                {
                    _checksum ^= buffer[i];
                }
            }
            else
            {
                for (var i = offset; i < count; i++)
                {
                    _checksum = (byte) (_checksum + buffer[i]);
                }
            }
        }

        /// <summary>
        ///     This is called by the framework to retrieve the final value from computation.
        /// </summary>
        /// <returns></returns>
        protected override object ComputeFinal()
        {
            switch (Mode)
            {
                case ChecksumMode.TwosComplement:
                    return (byte) (0x100 - _checksum);
                case ChecksumMode.Modulo256:
                    return (byte) (_checksum % 256);
                case ChecksumMode.Xor:
                    return _checksum;
                default:
                    throw new ArgumentException();
            }
        }
    }
}