using System;

namespace BinarySerialization
{
    internal static class Bytes
    {
        /// <summary>
        /// Flip the bytes in an UInt16.
        /// </summary>
        /// <param name="value">The value to flip.</param>
        /// <returns>An UInt16 with flipped bytes.</returns>
        public static UInt16 Reverse(UInt16 value)
        {
            return (UInt16) (((value & 0xFF00) >> 8) |
                             ((value & 0x00FF) << 8));
        }

        /// <summary>
        /// Flip the bytes in an Int16.
        /// </summary>
        /// <param name="value">The value to flip.</param>
        /// <returns>An Int16 with flipped bytes.</returns>
        public static Int16 Reverse(Int16 value)
        {
            return (Int16)Reverse((UInt16)value);
        }

        /// <summary>
        /// Reverse the bytes in an UInt32.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>An UInt32 with reversed bytes.</returns>
        public static UInt32 Reverse(UInt32 value)
        {
            return
                ((value & 0xff000000) >> 24) |
                ((value & 0x00ff0000) >> 8) |
                ((value & 0x0000ff00) << 8) |
                ((value & 0x000000ff) << 24);
        }

        /// <summary>
        /// Reverse the bytes in an Int32.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>An Int32 with reversed bytes.</returns>
        public static Int32 Reverse(Int32 value)
        {
            return (Int32) Reverse((UInt32) value);
        }

        /// <summary>
        /// Reverse the bytes in an UInt64.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>An UInt64 with reversed bytes.</returns>
        public static UInt64 Reverse(UInt64 value)
        {
            return
                (((value & 0xFF00000000000000) >> 56) |
                 ((value & 0x00FF000000000000) >> 40) |
                 ((value & 0x0000FF0000000000) >> 24) |
                 ((value & 0x000000FF00000000) >> 8) |
                 ((value & 0x00000000FF000000) << 8) |
                 ((value & 0x0000000000FF0000) << 24) |
                 ((value & 0x000000000000FF00) << 40) |
                 ((value & 0x00000000000000FF) << 56));
        }

        /// <summary>
        /// Reverse the bytes in an Int64.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>An Int64 with reversed bytes.</returns>
        public static Int64 Reverse(Int64 value)
        {
            return (Int64) Reverse((UInt64) value);
        }

        /// <summary>
        /// Reverse the bytes in a Single.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>A Single with reversed bytes.</returns>
        public static Single Reverse(Single value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Reverse the bytes in a Double.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>A Double with reversed bytes.</returns>
        public static Double Reverse(Double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }
    }
}