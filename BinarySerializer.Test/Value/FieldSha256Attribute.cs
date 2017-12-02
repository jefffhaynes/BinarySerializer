using System.Security.Cryptography;

namespace BinarySerialization.Test.Value
{
    public class FieldSha256Attribute : FieldValueAttributeBase
    {
        private readonly IncrementalHash _sha = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

        public FieldSha256Attribute(string valuePath) : base(valuePath)
        {
        }

        protected override void Reset(BinarySerializationContext context)
        {
            _sha.GetHashAndReset();
        }

        protected override void Compute(byte[] buffer, int offset, int count)
        {
            _sha.AppendData(buffer, offset, count);
        }

        protected override object ComputeFinal()
        {
            return _sha.GetHashAndReset();
        }
    }
}