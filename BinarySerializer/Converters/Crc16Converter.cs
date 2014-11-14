using System.Security.Cryptography;

namespace BinarySerialization.Converters
{
    public class Crc16Converter : CrcBaseConverter
    {
        protected override HashAlgorithm CrcAlgorithm
        {
            get { return new Crc16(); }
        }
    }
}
