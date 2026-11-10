using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.SourceGeneration.Test
{
    /// <summary>
    ///     Verifies that source-generated registration produces identical serialization behavior to
    ///     the reflection path (golden bytes) and that the generated accessors have the exact same
    ///     semantics as the reflection accessors (null handling, init-only fallback, and so on).
    /// </summary>
    [TestClass]
    public class GeneratedRegistrationTests
    {
        #region Registry wiring

        [TestMethod]
        public void RegistersPropertyAccessors()
        {
            Assert.IsTrue(GeneratedTypeRegistry.TryGetAccessors(typeof(SimplePacket), "A", out var getter, out var setter));
            Assert.IsNotNull(getter);
            Assert.IsNotNull(setter);

            var packet = new SimplePacket {A = 42};
            Assert.AreEqual((ushort) 42, getter(packet));

            setter(packet, (ushort) 7);
            Assert.AreEqual((ushort) 7, packet.A);
        }

        [TestMethod]
        public void RegistersFieldAccessors()
        {
            Assert.IsTrue(GeneratedTypeRegistry.TryGetAccessors(typeof(FieldPacket), "Id", out var getter, out var setter));
            Assert.IsNotNull(getter);
            Assert.IsNotNull(setter);

            var packet = new FieldPacket();
            setter(packet, (ushort) 5);
            Assert.AreEqual((ushort) 5, packet.Id);
            Assert.AreEqual((ushort) 5, getter(packet));
        }

        [TestMethod]
        public void SkipsInitOnlyMembersSoReflectionFallbackHandlesThem()
        {
            Assert.IsFalse(GeneratedTypeRegistry.TryGetAccessors(typeof(InitPacket), "A", out _, out _));
        }

        [TestMethod]
        public void GetOnlyPropertyRegistersGetterWithNullSetter()
        {
            Assert.IsTrue(GeneratedTypeRegistry.TryGetAccessors(typeof(AccessorProbe), "ReadOnlyValue",
                out var getter, out var setter));
            Assert.IsNotNull(getter);
            Assert.IsNull(setter);

            var probe = new AccessorProbe();
            Assert.AreEqual(9, getter(probe));
        }

        [TestMethod]
        public void PropertySetterSkipsNullValues()
        {
            Assert.IsTrue(GeneratedTypeRegistry.TryGetAccessors(typeof(SimplePacket), "B", out _, out var setter));

            var packet = new SimplePacket {B = 3};
            setter(packet, null);
            Assert.AreEqual((byte) 3, packet.B);
        }

        [TestMethod]
        public void FieldSetterAssignsNullValues()
        {
            Assert.IsTrue(GeneratedTypeRegistry.TryGetAccessors(typeof(FieldNullProbe), "Label", out _, out var setter));

            var probe = new FieldNullProbe {Label = "x"};
            setter(probe, null);
            Assert.IsNull(probe.Label);
        }

        [TestMethod]
        public void RegistersParameterlessFactory()
        {
            Assert.IsTrue(GeneratedTypeRegistry.TryGetParameterlessConstructor(typeof(SimplePacket), out var factory));
            Assert.IsNotNull(factory);
            Assert.IsInstanceOfType(factory(), typeof(SimplePacket));
        }

        [TestMethod]
        public void ImmutableTypeHasNoParameterlessFactory()
        {
            Assert.IsFalse(GeneratedTypeRegistry.TryGetParameterlessConstructor(typeof(ImmutablePoint), out _));
        }

        [TestMethod]
        public void RootMarkerRegistersAttributeFreeType()
        {
            Assert.IsTrue(GeneratedTypeRegistry.TryGetAccessors(typeof(RootOnlyPacket), "Value", out var getter, out _));
            Assert.IsNotNull(getter);
        }

        [TestMethod]
        public void NestedTypeIsRegistered()
        {
            Assert.IsTrue(GeneratedTypeRegistry.TryGetAccessors(typeof(NestedOuter.NestedDto), "Value", out _, out _));
        }

        #endregion

        #region Golden bytes (must match the reflection path exactly)

        [TestMethod]
        public void GoldenSimplePacket()
        {
            Roundtrip("SimplePacket", new byte[] {0x34, 0x12, 0xAB}, typeof(SimplePacket),
                () => new SimplePacket {A = 0x1234, B = 0xAB},
                value =>
                {
                    var packet = (SimplePacket) value;
                    Assert.AreEqual((ushort) 0x1234, packet.A);
                    Assert.AreEqual((byte) 0xAB, packet.B);
                });
        }

        [TestMethod]
        public void GoldenFieldPacket()
        {
            Roundtrip("FieldPacket", new byte[] {0x34, 0x12, 0xAB}, typeof(FieldPacket),
                () => new FieldPacket {Id = 0x1234, Flags = 0xAB},
                value =>
                {
                    var packet = (FieldPacket) value;
                    Assert.AreEqual((ushort) 0x1234, packet.Id);
                    Assert.AreEqual((byte) 0xAB, packet.Flags);
                });
        }

        [TestMethod]
        public void GoldenImmutablePoint()
        {
            Roundtrip("ImmutablePoint", new byte[] {0x0A, 0x00, 0x00, 0x00, 0xEC, 0xFF, 0xFF, 0xFF},
                typeof(ImmutablePoint), () => new ImmutablePoint(10, -20),
                value =>
                {
                    var point = (ImmutablePoint) value;
                    Assert.AreEqual(10, point.X);
                    Assert.AreEqual(-20, point.Y);
                });
        }

        [TestMethod]
        public void GoldenEnumListPacket()
        {
            var golden = new byte[] {0x02, 0x02, 0x02, 0x01, 0x01, 0x04, 0x03};

            Roundtrip("EnumListPacket", golden, typeof(EnumListPacket),
                () => new EnumListPacket
                {
                    Count = 2,
                    Items = new List<EnumItem>
                    {
                        new EnumItem {Kind = ItemKind.Beta, Value = 0x0102},
                        new EnumItem {Kind = ItemKind.Alpha, Value = 0x0304}
                    }
                },
                value =>
                {
                    var packet = (EnumListPacket) value;
                    Assert.AreEqual((byte) 2, packet.Count);
                    Assert.AreEqual(2, packet.Items.Count);
                    Assert.AreEqual(ItemKind.Beta, packet.Items[0].Kind);
                    Assert.AreEqual((ushort) 0x0102, packet.Items[0].Value);
                    Assert.AreEqual(ItemKind.Alpha, packet.Items[1].Kind);
                    Assert.AreEqual((ushort) 0x0304, packet.Items[1].Value);
                });
        }

        [TestMethod]
        public void GoldenSubtypePacket()
        {
            Roundtrip("SubtypePacket", new byte[] {0x02, 0x44, 0x33, 0x22, 0x11}, typeof(SubtypePacket),
                () => new SubtypePacket {Type = 2, Payload = new FrameB {B = 0x11223344}},
                value =>
                {
                    var packet = (SubtypePacket) value;
                    Assert.AreEqual((byte) 2, packet.Type);
                    Assert.IsInstanceOfType(packet.Payload, typeof(FrameB));
                    Assert.AreEqual(0x11223344, ((FrameB) packet.Payload).B);
                });
        }

        [TestMethod]
        public void GoldenInitPacket()
        {
            Roundtrip("InitPacket", new byte[] {0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00}, typeof(InitPacket),
                () => new InitPacket {A = 1, B = 2},
                value =>
                {
                    var packet = (InitPacket) value;
                    Assert.AreEqual(1, packet.A);
                    Assert.AreEqual(2, packet.B);
                });
        }

        [TestMethod]
        public void GoldenDerivedPacket()
        {
            Roundtrip("DerivedPacket", new byte[] {0x01, 0x02}, typeof(DerivedPacket),
                () => new DerivedPacket {BaseField = 1, DerivedField = 2},
                value =>
                {
                    var packet = (DerivedPacket) value;
                    Assert.AreEqual((byte) 1, packet.BaseField);
                    Assert.AreEqual((byte) 2, packet.DerivedField);
                });
        }

        [TestMethod]
        public void GoldenStringPacket()
        {
            Roundtrip("StringPacket", new byte[] {0x02, 0x00, 0x41, 0x42}, typeof(StringPacket),
                () => new StringPacket {Length = 2, Text = "AB"},
                value =>
                {
                    var packet = (StringPacket) value;
                    Assert.AreEqual((ushort) 2, packet.Length);
                    Assert.AreEqual("AB", packet.Text);
                });
        }

        [TestMethod]
        public void GoldenPrimitiveListPacket()
        {
            Roundtrip("PrimitiveListPacket", new byte[] {0x03, 0x10, 0x20, 0x30}, typeof(PrimitiveListPacket),
                () => new PrimitiveListPacket {Count = 3, Data = new List<byte> {0x10, 0x20, 0x30}},
                value =>
                {
                    var packet = (PrimitiveListPacket) value;
                    Assert.AreEqual((byte) 3, packet.Count);
                    CollectionAssert.AreEqual(new List<byte> {0x10, 0x20, 0x30}, packet.Data);
                });
        }

        [TestMethod]
        public void GoldenNestedOuter()
        {
            Roundtrip("NestedOuter", new byte[] {0x07}, typeof(NestedOuter),
                () => new NestedOuter {Inner = new NestedOuter.NestedDto {Value = 7}},
                value =>
                {
                    var outer = (NestedOuter) value;
                    Assert.AreEqual((byte) 7, outer.Inner.Value);
                });
        }

        [TestMethod]
        public void GoldenRootOnlyPacket()
        {
            Roundtrip("RootOnlyPacket", new byte[] {0x07, 0x00, 0x00, 0x00}, typeof(RootOnlyPacket),
                () => new RootOnlyPacket {Value = 7},
                value => Assert.AreEqual(7, ((RootOnlyPacket) value).Value));
        }

        [TestMethod]
        public void GoldenAccessorProbeSerialize()
        {
            // Contains a get-only property outside the constructor, which is serialize-only.
            var golden = new byte[] {0x05, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00};
            var serializer = new BinarySerializer();
            var probe = new AccessorProbe {Value = 5, Number = 6};

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, probe);
                CollectionAssert.AreEqual(golden, stream.ToArray());
            }
        }

        #endregion

        #region Helpers

        private static void Roundtrip(string name, byte[] golden, Type type, Func<object> create, Action<object> verify)
        {
            var serializer = new BinarySerializer();

            byte[] actual;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, create());
                actual = stream.ToArray();
            }

            CollectionAssert.AreEqual(golden, actual, $"{name}: serialize mismatch");

            var deserialized = serializer.Deserialize(golden, type);
            verify(deserialized);

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, deserialized);
                CollectionAssert.AreEqual(golden, stream.ToArray(), $"{name}: roundtrip mismatch");
            }
        }

        #endregion
    }
}
