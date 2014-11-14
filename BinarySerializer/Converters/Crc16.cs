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
    /// <summary>Implements a 16-bits cyclic redundancy check (CRC) hash algorithm.</summary>
    /// <remarks>
    ///     This class is not intended to be used for security purposes. For security applications use MD5, SHA1, SHA256, SHA384,
    ///     or SHA512 in the System.Security.Cryptography namespace.
    /// </remarks>
    public sealed class Crc16 : HashAlgorithm
    {
        static Crc16()
        {
            Crc16TablesCache = Hashtable.Synchronized(new Hashtable());
            DefaultCrc = new Crc16();
        }

        /// <summary>
        ///     Creates a CRC16 object using the <see cref="DefaultPolynomial" />.
        /// </summary>
        public Crc16() : this(DefaultPolynomial)
        {
        }

        /// <summary>Creates a CRC16 object using the specified polynomial.</summary>
        [CLSCompliant(false)]
        public Crc16(ushort polynomial)
        {
            HashSizeValue = 16;
            _crc16Table = (ushort[]) Crc16TablesCache[polynomial];
            if (_crc16Table == null)
            {
                _crc16Table = _buildCRC16Table(polynomial);
                Crc16TablesCache.Add(polynomial, _crc16Table);
            }
            Initialize();
        }

        // static constructor

        /// <summary>Gets the default polynomial.</summary>
        [CLSCompliant(false)] public static readonly ushort DefaultPolynomial = 0x8408; // Bit reversion of 0xA001;

        /// <summary>Initializes an implementation of HashAlgorithm.</summary>
        public override void Initialize()
        {
            _crc = 0;
        }

        /// <summary>Routes data written to the object into the hash algorithm for computing the hash.</summary>
        protected override void HashCore(byte[] buffer, int offset, int count)
        {
            foreach (byte b in buffer)
            {
                var index = (byte) (_crc ^ b);
                _crc = (ushort) ((_crc >> 8) ^ _crc16Table[index]);
            }
        }

        /// <summary>Finalizes the hash computation after the last data is processed by the cryptographic stream object.</summary>
        protected override byte[] HashFinal()
        {
            var finalHash = new byte[2];
            var finalCrc = (ushort) (_crc ^ AllOnes);

            finalHash[0] = (byte) ((finalCrc >> 0) & 0xFF);
            finalHash[1] = (byte) ((finalCrc >> 8) & 0xFF);

            return finalHash;
        }

        /// <summary>
        ///     Computes the CRC16 value for the given ASCII string using the <see cref="DefaultPolynomial" />.
        /// </summary>
        public static short Compute(string asciiString)
        {
            DefaultCrc.Initialize();
            return ToInt16(DefaultCrc.ComputeHash(asciiString));
        }

        /// <summary>
        ///     Computes the CRC16 value for the given input stream using the <see cref="DefaultPolynomial" />.
        /// </summary>
        public static short Compute(Stream inputStream)
        {
            DefaultCrc.Initialize();
            return ToInt16(DefaultCrc.ComputeHash(inputStream));
        }

        /// <summary>
        ///     Computes the CRC16 value for the input data using the <see cref="DefaultPolynomial" />.
        /// </summary>
        public static short Compute(byte[] buffer)
        {
            DefaultCrc.Initialize();
            return ToInt16(DefaultCrc.ComputeHash(buffer));
        }

        /// <summary>
        ///     Computes the hash value for the input data using the <see cref="DefaultPolynomial" />.
        /// </summary>
        public static short Compute(byte[] buffer, int offset, int count)
        {
            DefaultCrc.Initialize();
            return ToInt16(DefaultCrc.ComputeHash(buffer, offset, count));
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

        private const ushort AllOnes = 0xffff;
        private static readonly Crc16 DefaultCrc;
        private static readonly Hashtable Crc16TablesCache;
        private readonly ushort[] _crc16Table;
        private ushort _crc;

        // Builds a crc16 table given a polynomial
        private static ushort[] _buildCRC16Table(ushort polynomial)
        {
            // 256 values representing ASCII character codes. 
            var table = new ushort[256];
            for (ushort i = 0; i < table.Length; i++)
            {
                ushort value = 0;
                ushort temp = i;
                for (byte j = 0; j < 8; j++)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort) ((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
            return table;
        }

        private static short ToInt16(byte[] buffer)
        {
            return BitConverter.ToInt16(buffer, 0);
        }
    }
}