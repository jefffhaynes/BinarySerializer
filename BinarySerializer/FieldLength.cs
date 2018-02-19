using System;

namespace BinarySerialization
{
    public class FieldLength : IEquatable<FieldLength>
    {
        private const int BitsPerByte = 8;

        public static readonly FieldLength Zero = new FieldLength(0);
        public static readonly FieldLength MaxValue = new FieldLength(ulong.MaxValue);

        public FieldLength(ulong byteCount, int bitCount = 0)
        {
            ByteCount = byteCount + (ulong) bitCount / BitsPerByte;
            BitCount = bitCount % BitsPerByte;
        }

        public FieldLength(long byteCount, int bitCount = 0) : this(Convert.ToUInt64(byteCount), bitCount)
        {
        }

        public FieldLength(int byteCount, int bitCount = 0) : this(Convert.ToUInt64(byteCount), bitCount)
        {
        }

        public static FieldLength FromBitCount(int count)
        {
            return new FieldLength(0, count);
        }

        public ulong ByteCount { get; }

        public int BitCount { get; }

        public ulong TotalBitCount => ByteCount * BitsPerByte + (ulong) BitCount;

        public ulong TotalByteCount => BitCount > 0 ? ByteCount + 1 : ByteCount;

        public bool Equals(FieldLength other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ByteCount == other.ByteCount && BitCount == other.BitCount;
        }

        public static implicit operator FieldLength(ulong byteCount)
        {
            return new FieldLength(byteCount);
        }

        public static implicit operator FieldLength(long byteCount)
        {
            return new FieldLength(byteCount);
        }

        public static FieldLength operator +(FieldLength l1, FieldLength l2)
        {
            return new FieldLength(l1.ByteCount + l2.ByteCount, l1.BitCount + l2.BitCount);
        }

        public static FieldLength operator -(FieldLength l1, FieldLength l2)
        {
            return FromBitCount((int)(l1.TotalBitCount - l2.TotalBitCount));
        }

        public static FieldLength operator %(FieldLength l1, FieldLength l2)
        {
            return FromBitCount((int) l1.TotalBitCount % (int) l2.TotalBitCount);
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
            if (l1.ByteCount > l2.ByteCount) return true;

            if (l1.ByteCount < l2.ByteCount) return false;

            if (l1.BitCount > l2.BitCount) return true;

            if (l1.BitCount < l2.BitCount) return false;

            return false;
        }

        public static bool operator <(FieldLength l1, FieldLength l2)
        {
            if (l1.ByteCount < l2.ByteCount) return true;

            if (l1.ByteCount > l2.ByteCount) return false;

            if (l1.BitCount < l2.BitCount) return true;

            if (l1.BitCount > l2.BitCount) return false;

            return false;
        }

        public static bool operator >=(FieldLength l1, FieldLength l2)
        {
            if (l1.ByteCount > l2.ByteCount) return true;

            if (l1.ByteCount < l2.ByteCount) return false;

            if (l1.BitCount > l2.BitCount) return true;

            if (l1.BitCount < l2.BitCount) return false;

            return true;
        }

        public static bool operator <=(FieldLength l1, FieldLength l2)
        {
            if (l1.ByteCount < l2.ByteCount) return true;

            if (l1.ByteCount > l2.ByteCount) return false;

            if (l1.BitCount < l2.BitCount) return true;

            if (l1.BitCount > l2.BitCount) return false;

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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((FieldLength) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ByteCount.GetHashCode() * 397) ^ BitCount;
            }
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