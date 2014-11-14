using System.Security.Cryptography;

namespace BinarySerialization.Converters
{
    public class Crc32Converter : CrcBaseConverter
    {
        protected override HashAlgorithm CrcAlgorithm
        {
            get { return new Crc32(); }
        }
    }
}
