// Tamir Khason http://khason.net/
//
// Released under MS-PL : 6-Apr-09

using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BinarySerialization.Converters
{
    /// <summary>Implements a 32-bits cyclic redundancy check (CRC) hash algorithm.</summary>
    /// <remarks>
    ///     This class is not intended to be used for security purposes. For security applications use MD5, SHA1, SHA256, SHA384,
    ///     or SHA512 in the System.Security.Cryptography namespace.
    /// </remarks>
    public sealed class Crc32 : HashAlgorithm
    {
        // Bitwise reflection of 0x04C11DB7;

        private const uint AllOnes = 0xffffffff;

        /// <summary>Gets the default polynomial (used in WinZip, Ethernet, etc.)</summary>
        /// <remarks>The default polynomial is a bit-reflected version of the standard polynomial 0x04C11DB7 used by WinZip, Ethernet, etc.</remarks>
        [CLSCompliant(false)] public static readonly uint DefaultPolynomial = 0xEDB88320;

        private static readonly Crc32 DefaultCrc;
        private static readonly Hashtable Crc32TablesCache;
        private readonly uint[] _crc32Table;
        private uint _crc;

        static Crc32()
        {
            Crc32TablesCache = Hashtable.Synchronized(new Hashtable());
            DefaultCrc = new Crc32();
        }

        /// <summary>
        ///     Creates a CRC32 object using the <see cref="DefaultPolynomial" />.
        /// </summary>
        public Crc32() : this(DefaultPolynomial)
        {
        }

        /// <summary>Creates a CRC32 object using the specified polynomial.</summary>
        /// <remarks>
        ///     The polynomical should be supplied in its bit-reflected form. <see cref="DefaultPolynomial" />.
        /// </remarks>
        [CLSCompliant(false)]
        public Crc32(uint polynomial)
        {
            HashSizeValue = 32;
            _crc32Table = (uint[]) Crc32TablesCache[polynomial];
            if (_crc32Table == null)
            {
                _crc32Table = _buildCRC32Table(polynomial);
                Crc32TablesCache.Add(polynomial, _crc32Table);
            }
            Initialize();
        }

        // static constructor

        /// <summary>Initializes an implementation of HashAlgorithm.</summary>
        public override void Initialize()
        {
            _crc = AllOnes;
        }

        /// <summary>Routes data written to the object into the hash algorithm for computing the hash.</summary>
        protected override void HashCore(byte[] buffer, int offset, int count)
        {
            for (int i = offset; i < count; i++)
            {
                ulong ptr = (_crc & 0xFF) ^ buffer[i];
                _crc >>= 8;
                _crc ^= _crc32Table[ptr];
            }
        }

        /// <summary>Finalizes the hash computation after the last data is processed by the cryptographic stream object.</summary>
        protected override byte[] HashFinal()
        {
            var finalHash = new byte[4];
            ulong finalCrc = _crc ^ AllOnes;

            finalHash[0] = (byte) ((finalCrc >> 0) & 0xFF);
            finalHash[1] = (byte) ((finalCrc >> 8) & 0xFF);
            finalHash[2] = (byte) ((finalCrc >> 16) & 0xFF);
            finalHash[3] = (byte) ((finalCrc >> 24) & 0xFF);

            return finalHash;
        }

        /// <summary>
        ///     Computes the CRC32 value for the given ASCII string using the <see cref="DefaultPolynomial" />.
        /// </summary>
        public static int Compute(string asciiString)
        {
            DefaultCrc.Initialize();
            return ToInt32(DefaultCrc.ComputeHash(asciiString));
        }

        /// <summary>
        ///     Computes the CRC32 value for the given input stream using the <see cref="DefaultPolynomial" />.
        /// </summary>
        public static int Compute(Stream inputStream)
        {
            DefaultCrc.Initialize();
            return ToInt32(DefaultCrc.ComputeHash(inputStream));
        }

        /// <summary>
        ///     Computes the CRC32 value for the input data using the <see cref="DefaultPolynomial" />.
        /// </summary>
        public static int Compute(byte[] buffer)
        {
            DefaultCrc.Initialize();
            return ToInt32(DefaultCrc.ComputeHash(buffer));
        }

        /// <summary>
        ///     Computes the hash value for the input data using the <see cref="DefaultPolynomial" />.
        /// </summary>
        public static int Compute(byte[] buffer, int offset, int count)
        {
            DefaultCrc.Initialize();
            return ToInt32(DefaultCrc.ComputeHash(buffer, offset, count));
        }

        /// <summary>Computes the hash value for the given ASCII string.</summary>
        /// <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
        public byte[] ComputeHash(string asciiString)
        {
            byte[] rawBytes = Encoding.ASCII.GetBytes(asciiString);
            return ComputeHash(rawBytes);
        }

        /// <summary>Computes the hash value for the given input stream.</summary>
        /// <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
        public new byte[] ComputeHash(Stream inputStream)
        {
            var buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = inputStream.Read(buffer, 0, 4096)) > 0)
            {
                HashCore(buffer, 0, bytesRead);
            }
            return HashFinal();
        }

        /// <summary>Computes the hash value for the input data.</summary>
        /// <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
        public new byte[] ComputeHash(byte[] buffer)
        {
            return ComputeHash(buffer, 0, buffer.Length);
        }

        /// <summary>Computes the hash value for the input data.</summary>
        /// <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
        public new byte[] ComputeHash(byte[] buffer, int offset, int count)
        {
            HashCore(buffer, offset, count);
            return HashFinal();
        }

        // Builds a crc32 table given a polynomial
        private static uint[] _buildCRC32Table(uint polynomial)
        {
            var table = new uint[256];

            // 256 values representing ASCII character codes. 
            for (int i = 0; i < 256; i++)
            {
                var crc = (uint) i;
                for (int j = 8; j > 0; j--)
                {
                    if ((crc & 1) == 1)
                        crc = (crc >> 1) ^ polynomial;
                    else
                        crc >>= 1;
                }
                table[i] = crc;
            }

            return table;
        }

        private static int ToInt32(byte[] buffer)
        {
            return BitConverter.ToInt32(buffer, 0);
        }
    }
}