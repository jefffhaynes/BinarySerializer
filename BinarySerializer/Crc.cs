using System.Collections.Generic;

namespace BinarySerialization
{
    internal abstract class Crc<T>
    {
        private static readonly Dictionary<T, T[]> Tables = new Dictionary<T, T[]>();
        
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object TableLock = new object();

        private readonly T _initialValue;
        private readonly T[] _table;
        private T _crc;

        protected Crc(T polynomial, T initialValue)
        {
            _initialValue = initialValue;

            Reset();

            lock (TableLock)
            {
                if (Tables.TryGetValue(polynomial, out _table))
                {
                    return;
                }

                _table = BuildTable(polynomial);

                Tables.Add(polynomial, _table);
            }
        }

        public bool IsDataReflected { get; set; }
        public bool IsRemainderReflected { get; set; }
        public T FinalXor { get; set; }

        protected abstract int Width { get; }

        public void Reset()
        {
            _crc = _initialValue;
        }

        public void Compute(byte[] buffer, int offset, int count)
        {
            var remainder = ToUInt32(_crc);

            for (var i = offset; i < count; ++i)
            {
                var b = buffer[i];

                if (IsDataReflected)
                {
                    b = (byte) Reflect(b, 8);
                }

                var data = (byte) (b ^ (remainder >> (Width - 8)));

                remainder = ToUInt32(_table[data]) ^ (remainder << 8);
            }

            _crc = FromUInt32(remainder);
        }


        public T ComputeFinal()
        {
            var crc = _crc;

            if (IsRemainderReflected)
            {
                crc = FromUInt32(Reflect(ToUInt32(crc), Width));
            }

            return FromUInt32(ToUInt32(crc) ^ ToUInt32(FinalXor));
        }


        protected abstract uint ToUInt32(T value);
        protected abstract T FromUInt32(uint value);

        private T[] BuildTable(T polynomial)
        {
            var table = new T[256];

            var poly = ToUInt32(polynomial);

            var padWidth = Width - 8;

            var topBit = 1 << (Width - 1);
            
            // Compute the remainder of each possible dividend.
            for (uint dividend = 0; dividend < table.Length; dividend++)
            {
                // Start with the dividend followed by zeros.
                var remainder = dividend << padWidth;
                
                // Perform modulo-2 division, a bit at a time.
                for (uint bit = 8; bit > 0; bit--)
                {
                    // Try to divide the current data bit.
                    if ((remainder & topBit) != 0)
                    {
                        remainder = (remainder << 1) ^ poly;
                    }
                    else
                    {
                        remainder = remainder << 1;
                    }
                }

                // Store the result into the table.
                table[dividend] = FromUInt32(remainder);
            }

            return table;
        }

        private static uint Reflect(uint value, int bitCount)
        {
            uint reflection = 0;

            // Reflect the data about the center bit.
            for (int bit = 0; bit < bitCount; ++bit)
            {
                // If the LSB bit is set, set the reflection of it.
                if ((value & 0x01) != 0)
                {
                    reflection |= (uint) (1 << (bitCount - 1 - bit));
                }

                value = value >> 1;
            }

            return reflection;
        }
    }
}