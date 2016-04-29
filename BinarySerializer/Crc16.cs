using System.Collections.Generic;

namespace BinarySerialization
{
    internal class Crc16
    {
        private static readonly Dictionary<ushort, ushort[]> Tables = new Dictionary<ushort, ushort[]>();
        private static readonly object TableLock = new object();

        private readonly ushort[] _table;
        private readonly ushort _initialValue;

        private ushort _crc;

        public Crc16(ushort polynomial, ushort initialValue)
        {
            _initialValue = initialValue;

            lock (TableLock)
            {
                if (Tables.TryGetValue(polynomial, out _table))
                    return;

                _table = BuildTable(polynomial);
                Tables.Add(polynomial, _table);
            }

            Reset();
        }

        public void Reset()
        {
            _crc = _initialValue;
        }

        public void Compute(byte[] buffer, int offset, int count)
        {
            for (int i = offset; i < count; ++i)
            {
                _crc = (ushort)((_crc << 8) ^ _table[((_crc >> 8) ^ (0xff & buffer[i]))]);
            }
        }

        public ushort ComputeFinal()
        {
            return _crc;
        }

        private static ushort[] BuildTable(ushort polynomial)
        {
            var table = new ushort[256];
            ushort temp, a;
            for (int i = 0; i < table.Length; ++i)
            {
                temp = 0;
                a = (ushort)(i << 8);
                for (int j = 0; j < 8; ++j)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                    {
                        temp = (ushort)((temp << 1) ^ polynomial);
                    }
                    else
                    {
                        temp <<= 1;
                    }
                    a <<= 1;
                }
                table[i] = temp;
            }
            return table;
        }
    }
}
