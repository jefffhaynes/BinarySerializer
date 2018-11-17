using System;

namespace BinarySerialization
{
    internal static class Bytes
    {
        /// <summary>
        ///     Flip the bytes in an UInt16.
        /// </summary>
        /// <param name="value">The value to flip.</param>
        /// <returns>An UInt16 with flipped bytes.</returns>
        public static ushort Reverse(ushort value)
        {
            return (ushort) (((value & 0xFF00) >> 8) |
                             ((value & 0x00FF) << 8));
        }

        /// <summary>
        ///     Flip the bytes in an Int16.
        /// </summary>
        /// <param name="value">The value to flip.</param>
        /// <returns>An Int16 with flipped bytes.</returns>
        public static short Reverse(short value)
        {
            return (short) Reverse((ushort) value);
        }

        /// <summary>
        ///     Reverse the bytes in an UInt32.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>An UInt32 with reversed bytes.</returns>
        public static uint Reverse(uint value)
        {
            return
                ((value & 0xff000000) >> 24) |
                ((value & 0x00ff0000) >> 8) |
                ((value & 0x0000ff00) << 8) |
                ((value & 0x000000ff) << 24);
        }

        /// <summary>
        ///     Reverse the bytes in an Int32.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>An Int32 with reversed bytes.</returns>
        public static int Reverse(int value)
        {
            return (int) Reverse((uint) value);
        }

        /// <summary>
        ///     Reverse the bytes in an UInt64.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>An UInt64 with reversed bytes.</returns>
        public static ulong Reverse(ulong value)
        {
            return
                ((value & 0xFF00000000000000) >> 56) |
                ((value & 0x00FF000000000000) >> 40) |
                ((value & 0x0000FF0000000000) >> 24) |
                ((value & 0x000000FF00000000) >> 8) |
                ((value & 0x00000000FF000000) << 8) |
                ((value & 0x0000000000FF0000) << 24) |
                ((value & 0x000000000000FF00) << 40) |
                ((value & 0x00000000000000FF) << 56);
        }

        /// <summary>
        ///     Reverse the bytes in an Int64.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>An Int64 with reversed bytes.</returns>
        public static long Reverse(long value)
        {
            return (long) Reverse((ulong) value);
        }

        /// <summary>
        ///     Reverse the bytes in a Single.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>A Single with reversed bytes.</returns>
        public static float Reverse(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        ///     Reverse the bytes in a Double.
        /// </summary>
        /// <param name="value">The value to reverse.</param>
        /// <returns>A Double with reversed bytes.</returns>
        public static double Reverse(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }
    }
}