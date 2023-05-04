using System;

namespace BinarySerialization
{
    public class FieldLength : IEquatable<FieldLength>
    {
        private const int BitsPerByte = 8;

        public static readonly FieldLength Zero = new FieldLength(0);
        public static readonly FieldLength MaxValue = new FieldLength(long.MaxValue);

        public FieldLength(long byteCount)
        {
            ByteCount = byteCount;
            BitCount = 0;
        }

        public FieldLength(int byteCount) : this(Convert.ToInt64(byteCount))
        {
        }

        public FieldLength(long byteCount, long bitCount, BitOrder bitOrder)
        {
            ByteCount = byteCount + bitCount / BitsPerByte;
            BitCount = (int) bitCount % BitsPerByte;
            BitOrder = bitOrder;
        }

        public FieldLength(int byteCount, int bitCount, BitOrder bitOrder) : this(Convert.ToInt64(byteCount), bitCount, bitOrder)
        {
        }

        public long ByteCount { get; }

        public int BitCount { get; }

        public BitOrder BitOrder { get; }

        public long TotalBitCount => ByteCount * BitsPerByte + BitCount;

        public long TotalByteCount => BitCount > 0 ? ByteCount + 1 : ByteCount;

        public bool Equals(FieldLength other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ByteCount == other.ByteCount && BitCount == other.BitCount
                && ((BitOrder == other.BitOrder) || (BitCount==0 || other.BitCount==0));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((FieldLength) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ByteCount.GetHashCode() * 397) ^ BitCount.GetHashCode();
            }
        }

        public static FieldLength FromBitCount(int count, BitOrder bitOrder)
        {
            return new FieldLength(0, count, bitOrder);
        }

        public static implicit operator FieldLength(long byteCount)
        {
            return new FieldLength(byteCount);
        }

        public static FieldLength operator +(FieldLength l1, FieldLength l2)
        {
            return new FieldLength(l1.ByteCount + l2.ByteCount, l1.BitCount + l2.BitCount, l1.BitOrder);
        }

        public static FieldLength operator -(FieldLength l1, FieldLength l2)
        {
            return FromBitCount((int) (l1.TotalBitCount - l2.TotalBitCount), l1.BitOrder);
        }

        public static FieldLength operator %(FieldLength l1, FieldLength l2)
        {
            return FromBitCount((int) l1.TotalBitCount % (int) l2.TotalBitCount, l1.BitOrder);
        }

        public static bool operator ==(FieldLength l1, FieldLength l2)
        {
            return Equals(l1, l2);
        }

        public static bool operator !=(FieldLength l1, FieldLength l2)
        {
            return !Equals(l1, l2);
        }

        public static bool operator >(FieldLength l1, FieldLength l2)
        {
            if (l1.ByteCount > l2.ByteCount)
            {
                return true;
            }

            if (l1.ByteCount < l2.ByteCount)
            {
                return false;
            }

            if (l1.BitCount > l2.BitCount)
            {
                return true;
            }

            if (l1.BitCount < l2.BitCount)
            {
                return false;
            }

            return false;
        }

        public static bool operator <(FieldLength l1, FieldLength l2)
        {
            if (l1.ByteCount < l2.ByteCount)
            {
                return true;
            }

            if (l1.ByteCount > l2.ByteCount)
            {
                return false;
            }

            if (l1.BitCount < l2.BitCount)
            {
                return true;
            }

            if (l1.BitCount > l2.BitCount)
            {
                return false;
            }

            return false;
        }

        public static bool operator >=(FieldLength l1, FieldLength l2)
        {
            if (l1.ByteCount > l2.ByteCount)
            {
                return true;
            }

            if (l1.ByteCount < l2.ByteCount)
            {
                return false;
            }

            if (l1.BitCount > l2.BitCount)
            {
                return true;
            }

            if (l1.BitCount < l2.BitCount)
            {
                return false;
            }

            return true;
        }

        public static bool operator <=(FieldLength l1, FieldLength l2)
        {
            if (l1.ByteCount < l2.ByteCount)
            {
                return true;
            }

            if (l1.ByteCount > l2.ByteCount)
            {
                return false;
            }

            if (l1.BitCount < l2.BitCount)
            {
                return true;
            }

            if (l1.BitCount > l2.BitCount)
            {
                return false;
            }

            return true;
        }

        public static FieldLength Min(FieldLength a, FieldLength b)
        {
            return a < b ? a : b;
        }

        public static FieldLength Max(FieldLength a, FieldLength b)
        {
            return a > b ? a : b;
        }

        public override string ToString()
        {
            if (BitCount == 0)
            {
                return ByteCount.ToString();
            }

            var totalBitCount = TotalBitCount;
            return totalBitCount == 1 ? "1 bit" : $"{totalBitCount} bits";
        }
    }
}