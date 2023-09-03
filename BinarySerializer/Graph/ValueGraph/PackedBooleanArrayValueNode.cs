using BinarySerialization.Graph.TypeGraph;
using System;
using System.Collections.Generic;

namespace BinarySerialization.Graph.ValueGraph
{
    internal class PackedBooleanArrayValueNode : ValueNode
    {
        public override object Value
        {
            get { return Values; }
            set
            {
                if (!(value is bool[]))
                    throw new InvalidOperationException("Only Boolean Arrays are valid as values for a Packed Boolean Array node.");

                Values = (bool[])value;
            }
        }

        private bool[] Values;

        public PackedBooleanArrayValueNode(Node parent, string name, TypeNode typeNode) : base(parent, name, typeNode) { }

        internal override void DeserializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            var size = GetFieldCount() ?? (GetFieldLength() * 8) ?? (stream.AvailableForReading * 8);

            Values = new bool[size];

            if (size == 0)
                return;

            int index = -1;
            int bit = -1;
            byte currentByte = 0;

            while (++index < Values.Length)
            {
                if (bit < 0 || bit > 7)
                {
                    int read;
                    if (stream.IsAtLimit || (read = stream.ReadByte()) < 0)
                        throw new InvalidOperationException("Stream ended before all booleans were deserialized.");

                    currentByte = (byte)read;
                    // Big Endian = Read from MSB to LSB (X000 0000, 0X00 0000, ..., 0000 00X0, 0000 000X)
                    // Little Endian = Read from LSB to MSB (0000 000X, 0000 00X0, ..., 0X00 000, X000 0000)
                    bit = GetFieldEndianness() == Endianness.Big ? 7 : 0;
                }

                int mask = (byte)(1 << bit);
                Values[index] = (currentByte & mask) == mask;

                if (GetFieldEndianness() == Endianness.Big)
                    bit--;
                else
                    bit++;                
            }

        }

        internal override void SerializeOverride(BoundedStream stream, EventShuttle eventShuttle)
        {
            if (Values == null || Values.Length == 0)
                return;

            if (GetConstFieldCount() != null)
                Array.Resize(ref Values, (int)GetConstFieldCount().Value);
            if (GetConstFieldLength() != null)
                Array.Resize(ref Values, (int)GetConstFieldLength().Value * 8);

            int index = -1;

            // Big Endian = Write from MSB to LSB (X000 0000, 0X00 0000, ..., 0000 00X0, 0000 000X)
            // Little Endian = Write from LSB to MSB (0000 000X, 0000 00X0, ..., 0X00 000, X000 0000)
            int bit = GetFieldEndianness() == Endianness.Big ? 7 : 0;
            byte currentByte = 0;

            while (++index < Values.Length)
            {
                if (Values[index])
                    currentByte |= (byte)(1 << bit);

                if (GetFieldEndianness() == Endianness.Big)
                    bit--;
                else
                    bit++;

                if (bit < 0 || bit > 7 || index == Values.Length - 1)
                {
                    if (stream.IsAtLimit)
                        break;

                    stream.WriteByte(currentByte);
                    bit = GetFieldEndianness() == Endianness.Big ? 7 : 0;
                    currentByte = 0;
                }
            }
        }

        protected override long CountOverride() => Values?.Length ?? 0;
        protected override long MeasureOverride() => (long)Math.Ceiling((Values?.Length ?? 0) / 8.0);
    }
}
