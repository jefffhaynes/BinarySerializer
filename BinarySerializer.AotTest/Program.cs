using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using BinarySerialization;

namespace BinarySerialization.AotTest
{
    /// <summary>
    ///     Native AOT smoke tests for BinarySerializer.  Each scenario serializes a known object graph,
    ///     compares the bytes against golden values, deserializes the golden bytes back and verifies the
    ///     fields, then re-serializes to confirm stability.  Exits non-zero on any failure.
    ///     Run with "dotnet run" (JIT baseline) and "dotnet publish -p:PublishAot=true" + run (AOT).
    /// </summary>
    internal static class Program
    {
        private static int _failures;

        private static int Main()
        {
            RunScenario("S1 Properties (MagicMethods accessors)", S1Golden, typeof(Header), CreateS1, VerifyS1);
            RunScenario("S2 Fields (FieldInfo accessors)", S2Golden, typeof(FieldsPacket), CreateS2, VerifyS2);
            RunScenario("S3 Enum member (Enum.GetValuesAsUnderlyingType)", S3Golden, typeof(EnumPacket), CreateS3, VerifyS3);
            RunScenario("S4 FieldLength binding + byte[]", S4Golden, typeof(BlobPacket), CreateS4, VerifyS4);
            RunScenario("S5 List<T> + FieldCount binding", S5Golden, typeof(ListPacket), CreateS5, VerifyS5);
            RunScenario("S6 Nested object graph", S6Golden, typeof(Outer), CreateS6, VerifyS6);
            RunScenario("S7 Subtype attribute mapping", S7Golden, typeof(SubtypePacket), CreateS7, VerifyS7);
            RunScenario("S8 Immutable type (parameterized constructor)", S8Golden, typeof(Point), CreateS8, VerifyS8);
            RunScenario("S9 IValueConverter on binding", S9Golden, typeof(ConvertPacket), CreateS9, VerifyS9);
            RunScenario("S10 Primitive array + FieldCount", S10Golden, typeof(ArrayPacket), CreateS10, VerifyS10);
            RunScenario("S12 Primitive List<byte> (Activator + array factory)", S12Golden, typeof(PrimitiveListPacket), CreateS12, VerifyS12);
            RunContextScenarios();

            if (_failures > 0)
            {
                Console.Error.WriteLine($"FAILED: {_failures} scenario check(s) failed.");
                return 1;
            }

            Console.WriteLine("All AOT smoke scenarios passed.");
            return 0;
        }

        #region Scenario runner

        private static void RunScenario(string name, byte[] golden,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type,
            Func<object> create, Action<object> verify)
        {
            Console.WriteLine($"-- {name}");

            var serializer = new BinarySerializer();

            // 1. Serialize and compare with golden bytes.
            var value = create();
            byte[] actual;

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, value);
                actual = stream.ToArray();
            }

            CheckBytes($"{name}: serialize", golden, actual);

            // 2. Deserialize golden bytes and verify the object graph.
            object deserialized = null;

            try
            {
                deserialized = serializer.Deserialize(golden, type);
                verify(deserialized);
            }
            catch (Exception exception)
            {
                Fail($"{name}: deserialize/verify threw {exception}");
            }

            // 3. Re-serialize and confirm byte stability.
            if (deserialized != null)
            {
                using (var stream = new MemoryStream())
                {
                    serializer.Serialize(stream, deserialized);
                    CheckBytes($"{name}: roundtrip", golden, stream.ToArray());
                }
            }
        }

        private static void RunContextScenarios()
        {
            Console.WriteLine("-- S11 Serialization context binding");

            var serializer = new BinarySerializer();
            var packet = new ContextPacket {Head = 1, Tail = 2};
            var context = new ContextInfo {IncludeTail = true};

            byte[] actual;

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, packet, context);
                actual = stream.ToArray();
            }

            CheckBytes("S11: serialize with context", S11Golden, actual);

            var deserialized = (ContextPacket) serializer.Deserialize(S11Golden, typeof(ContextPacket), context);

            if (deserialized.Head != 1 || deserialized.Tail != 2)
            {
                Fail($"S11: deserialize fields wrong: head={deserialized.Head} tail={deserialized.Tail}");
            }

            // The tail is conditioned on the context; without it only the head is serialized.
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, new ContextPacket {Head = 1, Tail = 2}, new ContextInfo {IncludeTail = false});
                CheckBytes("S11: serialize without context condition", new byte[] {0x01}, stream.ToArray());
            }
        }

        private static void CheckBytes(string what, byte[] expected, byte[] actual)
        {
            if (actual.Length == expected.Length)
            {
                var equal = true;

                for (var i = 0; i < actual.Length; i++)
                {
                    if (actual[i] != expected[i])
                    {
                        equal = false;
                        break;
                    }
                }

                if (equal)
                {
                    return;
                }
            }

            Fail($"{what}: expected [{Hex(expected)}] but was [{Hex(actual)}]");
        }

        private static void CheckEqual(string what, object expected, object actual)
        {
            if (!Equals(expected, actual))
            {
                Fail($"{what}: expected <{expected}> but was <{actual}>");
            }
        }

        private static void Fail(string message)
        {
            _failures++;
            Console.Error.WriteLine("   FAIL " + message);
        }

        private static string Hex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", " ");
        }

        #endregion

        #region S1: properties (MagicMethods path)

        private static readonly byte[] S1Golden = {0x05, 0x00, 0x02, 0xE8, 0x03, 0x00, 0x00};

        public class Header
        {
            [FieldOrder(0)]
            public ushort Length { get; set; }

            [FieldOrder(1)]
            public byte Version { get; set; }

            [FieldOrder(2)]
            public int Sequence { get; set; }
        }

        private static object CreateS1()
        {
            return new Header {Length = 5, Version = 2, Sequence = 1000};
        }

        private static void VerifyS1(object value)
        {
            var header = (Header) value;
            CheckEqual("S1.Length", (ushort) 5, header.Length);
            CheckEqual("S1.Version", (byte) 2, header.Version);
            CheckEqual("S1.Sequence", 1000, header.Sequence);
        }

        #endregion

        #region S2: fields

        private static readonly byte[] S2Golden = {0x34, 0x12, 0xAB};

        public class FieldsPacket
        {
            [FieldOrder(0)]
            public ushort Id;

            [FieldOrder(1)]
            public byte Flags;
        }

        private static object CreateS2()
        {
            return new FieldsPacket {Id = 0x1234, Flags = 0xAB};
        }

        private static void VerifyS2(object value)
        {
            var packet = (FieldsPacket) value;
            CheckEqual("S2.Id", (ushort) 0x1234, packet.Id);
            CheckEqual("S2.Flags", (byte) 0xAB, packet.Flags);
        }

        #endregion

        #region S3: enum

        private static readonly byte[] S3Golden = {0x02, 0x02, 0x01};

        public enum Kind : byte
        {
            None = 0,
            Alpha = 1,
            Beta = 2
        }

        public class EnumPacket
        {
            [FieldOrder(0)]
            public Kind Kind { get; set; }

            [FieldOrder(1)]
            public ushort Value { get; set; }
        }

        private static object CreateS3()
        {
            return new EnumPacket {Kind = Kind.Beta, Value = 0x0102};
        }

        private static void VerifyS3(object value)
        {
            var packet = (EnumPacket) value;
            CheckEqual("S3.Kind", Kind.Beta, packet.Kind);
            CheckEqual("S3.Value", (ushort) 0x0102, packet.Value);
        }

        #endregion

        #region S4: field length binding

        private static readonly byte[] S4Golden = {0x03, 0x00, 0xAA, 0xBB, 0xCC, 0x7E};

        public class BlobPacket
        {
            [FieldOrder(0)]
            public ushort Length { get; set; }

            [FieldOrder(1)]
            [FieldLength(nameof(Length))]
            public byte[] Data { get; set; }

            [FieldOrder(2)]
            public byte Tail { get; set; }
        }

        private static object CreateS4()
        {
            return new BlobPacket {Length = 3, Data = new byte[] {0xAA, 0xBB, 0xCC}, Tail = 0x7E};
        }

        private static void VerifyS4(object value)
        {
            var packet = (BlobPacket) value;
            CheckEqual("S4.Length", (ushort) 3, packet.Length);
            CheckBytes("S4.Data", new byte[] {0xAA, 0xBB, 0xCC}, packet.Data);
            CheckEqual("S4.Tail", (byte) 0x7E, packet.Tail);
        }

        #endregion

        #region S5: list + field count

        private static readonly byte[] S5Golden =
        {
            0x02,
            0x01, 0x00, 0x64, 0x00, 0x00, 0x00,
            0x02, 0x00, 0xFF, 0xFF, 0xFF, 0xFF
        };

        public class Item
        {
            [FieldOrder(0)]
            public ushort Id { get; set; }

            [FieldOrder(1)]
            public int Value { get; set; }
        }

        public class ListPacket
        {
            [FieldOrder(0)]
            public byte Count { get; set; }

            [FieldOrder(1)]
            [FieldCount(nameof(Count))]
            public List<Item> Items { get; set; }
        }

        private static object CreateS5()
        {
            return new ListPacket
            {
                Count = 2,
                Items = new List<Item>
                {
                    new Item {Id = 1, Value = 100},
                    new Item {Id = 2, Value = -1}
                }
            };
        }

        private static void VerifyS5(object value)
        {
            var packet = (ListPacket) value;
            CheckEqual("S5.Count", (byte) 2, packet.Count);
            CheckEqual("S5.Items.Count", 2, packet.Items.Count);
            CheckEqual("S5.Items[0].Id", (ushort) 1, packet.Items[0].Id);
            CheckEqual("S5.Items[0].Value", 100, packet.Items[0].Value);
            CheckEqual("S5.Items[1].Id", (ushort) 2, packet.Items[1].Id);
            CheckEqual("S5.Items[1].Value", -1, packet.Items[1].Value);
        }

        #endregion

        #region S6: nested object

        private static readonly byte[] S6Golden = {0x01, 0x02, 0x00, 0x03, 0x04, 0x00, 0x00, 0x00};

        public class Outer
        {
            [FieldOrder(0)]
            public byte Tag { get; set; }

            [FieldOrder(1)]
            public Header Inner { get; set; }
        }

        private static object CreateS6()
        {
            return new Outer {Tag = 1, Inner = new Header {Length = 2, Version = 3, Sequence = 4}};
        }

        private static void VerifyS6(object value)
        {
            var outer = (Outer) value;
            CheckEqual("S6.Tag", (byte) 1, outer.Tag);
            CheckEqual("S6.Inner.Length", (ushort) 2, outer.Inner.Length);
            CheckEqual("S6.Inner.Version", (byte) 3, outer.Inner.Version);
            CheckEqual("S6.Inner.Sequence", 4, outer.Inner.Sequence);
        }

        #endregion

        #region S7: subtypes

        private static readonly byte[] S7Golden = {0x02, 0x44, 0x33, 0x22, 0x11};

        public abstract class Frame
        {
        }

        public class FrameA : Frame
        {
            [FieldOrder(0)]
            public ushort A { get; set; }
        }

        public class FrameB : Frame
        {
            [FieldOrder(0)]
            public int B { get; set; }
        }

        public class SubtypePacket
        {
            [FieldOrder(0)]
            public byte Type { get; set; }

            [FieldOrder(1)]
            [Subtype(nameof(Type), 1, typeof(FrameA))]
            [Subtype(nameof(Type), 2, typeof(FrameB))]
            public Frame Payload { get; set; }
        }

        private static object CreateS7()
        {
            return new SubtypePacket {Type = 2, Payload = new FrameB {B = 0x11223344}};
        }

        private static void VerifyS7(object value)
        {
            var packet = (SubtypePacket) value;
            CheckEqual("S7.Type", (byte) 2, packet.Type);
            var frameB = packet.Payload as FrameB;

            if (frameB == null)
            {
                Fail($"S7: expected FrameB but was {packet.Payload?.GetType().Name ?? "null"}");
                return;
            }

            CheckEqual("S7.Payload.B", 0x11223344, frameB.B);
        }

        #endregion

        #region S8: immutable type with parameterized constructor

        private static readonly byte[] S8Golden = {0x0A, 0x00, 0x00, 0x00, 0xEC, 0xFF, 0xFF, 0xFF};

        public class Point
        {
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            [FieldOrder(0)]
            public int X { get; }

            [FieldOrder(1)]
            public int Y { get; }
        }

        private static object CreateS8()
        {
            return new Point(10, -20);
        }

        private static void VerifyS8(object value)
        {
            var point = (Point) value;
            CheckEqual("S8.X", 10, point.X);
            CheckEqual("S8.Y", -20, point.Y);
        }

        #endregion

        #region S9: value converter

        private static readonly byte[] S9Golden = {0x02, 0x00, 0x41, 0x42, 0x43, 0x44};

        public class DoubledLengthConverter : IValueConverter
        {
            public object Convert(object value, object converterParameter, BinarySerializationContext context)
            {
                return System.Convert.ToInt32(value) * 2;
            }

            public object ConvertBack(object value, object converterParameter, BinarySerializationContext context)
            {
                return System.Convert.ToInt32(value) / 2;
            }
        }

        public class ConvertPacket
        {
            [FieldOrder(0)]
            public ushort HalfLength { get; set; }

            [FieldOrder(1)]
            [FieldLength(nameof(HalfLength), ConverterType = typeof(DoubledLengthConverter))]
            public string Field { get; set; }
        }

        private static object CreateS9()
        {
            return new ConvertPacket {HalfLength = 2, Field = "ABCD"};
        }

        private static void VerifyS9(object value)
        {
            var packet = (ConvertPacket) value;
            CheckEqual("S9.HalfLength", (ushort) 2, packet.HalfLength);
            CheckEqual("S9.Field", "ABCD", packet.Field);
        }

        #endregion

        #region S10: primitive array

        private static readonly byte[] S10Golden = {0x02, 0x07, 0x00, 0x00, 0x00, 0xF9, 0xFF, 0xFF, 0xFF};

        public class ArrayPacket
        {
            [FieldOrder(0)]
            public byte Count { get; set; }

            [FieldOrder(1)]
            [FieldCount(nameof(Count))]
            public int[] Values { get; set; }
        }

        private static object CreateS10()
        {
            return new ArrayPacket {Count = 2, Values = new[] {7, -7}};
        }

        private static void VerifyS10(object value)
        {
            var packet = (ArrayPacket) value;
            CheckEqual("S10.Count", (byte) 2, packet.Count);
            CheckEqual("S10.Values.Length", 2, packet.Values.Length);
            CheckEqual("S10.Values[0]", 7, packet.Values[0]);
            CheckEqual("S10.Values[1]", -7, packet.Values[1]);
        }

        #endregion

        #region S12: primitive list

        private static readonly byte[] S12Golden = {0x03, 0x10, 0x20, 0x30};

        public class PrimitiveListPacket
        {
            [FieldOrder(0)]
            public byte Count { get; set; }

            [FieldOrder(1)]
            [FieldCount(nameof(Count))]
            public List<byte> Data { get; set; }
        }

        private static object CreateS12()
        {
            return new PrimitiveListPacket {Count = 3, Data = new List<byte> {0x10, 0x20, 0x30}};
        }

        private static void VerifyS12(object value)
        {
            var packet = (PrimitiveListPacket) value;
            CheckEqual("S12.Count", (byte) 3, packet.Count);
            CheckEqual("S12.Data.Count", 3, packet.Data.Count);
            CheckEqual("S12.Data[0]", (byte) 0x10, packet.Data[0]);
            CheckEqual("S12.Data[1]", (byte) 0x20, packet.Data[1]);
            CheckEqual("S12.Data[2]", (byte) 0x30, packet.Data[2]);
        }

        #endregion

        #region S11: serialization context

        private static readonly byte[] S11Golden = {0x01, 0x02};

        [BinarySerializerRoot]
        public class ContextInfo
        {
            public bool IncludeTail { get; set; }
        }

        public class ContextPacket
        {
            [FieldOrder(0)]
            public byte Head { get; set; }

            [FieldOrder(1)]
            [SerializeWhen(nameof(ContextInfo.IncludeTail), true, RelativeSourceMode = RelativeSourceMode.SerializationContext)]
            public byte Tail { get; set; }
        }

        #endregion
    }
}
