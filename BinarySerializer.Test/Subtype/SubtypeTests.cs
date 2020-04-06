using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace BinarySerialization.Test.Subtype
{
    public class SubtypeTests : TestBase
    {
        [Fact]
        public void SubtypeTest()
        {
            var expected = new SubtypeClass {Field = new SubclassB {SomethingForClassB = 33}, Field2 = new SubclassA()};
            var actual = Roundtrip(expected);

            Assert.Equal(SubclassType.B, actual.Subtype);
            Assert.IsAssignableFrom<SubclassB>(actual.Field);
        }

        [Fact]
        public void SubSubtypeTest()
        {
            var expected = new SubtypeClass
            {
                Field = new SubSubclassC(3)
                {
                    SomeSuperStuff = 1,
                    SomethingForClassB = 2
                },
                Field2 = new SubclassA()
            };
            var actual = Roundtrip(expected);

            Assert.Equal(SubclassType.C, actual.Subtype);
            Assert.IsAssignableFrom<SubSubclassC>(actual.Field);
            Assert.Equal(actual.Field.SomeSuperStuff, expected.Field.SomeSuperStuff);
            Assert.Equal(((SubSubclassC) actual.Field).SomethingForClassB,
                ((SubSubclassC) expected.Field).SomethingForClassB);
            Assert.Equal(((SubSubclassC) actual.Field).SomethingForClassC,
                ((SubSubclassC) expected.Field).SomethingForClassC);
        }

        [Fact]
        public void RecoverableMissingSubtypeTest()
        {
            var expected = new RecoverableMissingSubtypeClass<SuperclassContainer>
            {
                Items = new List<SuperclassContainer>
                {
                    new SuperclassContainer {Value = new SubclassA()},
                    new SuperclassContainer {Value = new SubclassB()},
                    new SuperclassContainer {Value = new SubSubclassC(33)}
                }
            };

            var stream = new MemoryStream();

            Serializer.Serialize(stream, expected);

            stream.Position = 0;

            var actual =
                Serializer.Deserialize<RecoverableMissingSubtypeClass<SuperclassContainerWithMissingSubclass>>(stream);

            var actualItems = actual.Items;

            Assert.Equal(typeof (SubclassA), actualItems[0].Value.GetType());
            Assert.Null(actualItems[1].Value);
            Assert.Equal(typeof (SubSubclassC), actualItems[2].Value.GetType());
        }

        [Fact]
        public void MissingSubtypeTest()
        {
            var expected = new IncompleteSubtypeClass { Field = new SubclassB() };
#if TESTASYNC
            Assert.Throws<AggregateException>(() => Roundtrip(expected));
#else
            Assert.Throws<InvalidOperationException>(() => Roundtrip(expected));
#endif
        }

        [Fact]
        public void SubtypeDefaultTest()
        {
            var data = new byte[] {0x0, 0x1, 0x2, 0x3, 0x4, 0x5};
            var actual = Deserialize<DefaultSubtypeContainerClass>(data);
            Assert.Equal(typeof(DefaultSubtypeClass), actual.Value.GetType());
        }

        [Fact]
        public void InvalidSubtypeDefaultTest()
        {
            var data = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };
#if TESTASYNC
            Assert.Throws<AggregateException>(() =>
#else
            Assert.Throws<InvalidOperationException>(() =>
#endif
            Deserialize<InvalidDefaultSubtypeContainerClass>(data));

        }

        [Fact]
        public void DefaultSubtypeForwardTest()
        {
            var expected = new DefaultSubtypeContainerClass
            {
                Value = new SubclassA()
            };

            var actual = Roundtrip(expected);

            Assert.Equal(1, actual.Indicator);
            Assert.Equal(typeof(SubclassA), actual.Value.GetType());
        }

        [Fact]
        public void DefaultSubtypeAllowOnSerialize()
        {
            var expected = new DefaultSubtypeContainerClass
            {
                Indicator = 33,
                Value = new DefaultSubtypeClass()
            };

            var actual = Roundtrip(expected);

            Assert.Equal(33, actual.Indicator);
            Assert.Equal(typeof(DefaultSubtypeClass), actual.Value.GetType());
        }

        [Fact]
        public void AncestorSubtypeBindingTest()
        {
            var expected = new AncestorSubtypeBindingContainerClass
            {
                AncestorSubtypeBindingClass =
                    new AncestorSubtypeBindingClass
                    {
                        InnerClass = new AncestorSubtypeBindingInnerClass {Value = "hello"}
                    }
            };

            var actual = Roundtrip(expected);
            Assert.Equal(((AncestorSubtypeBindingClass) expected.AncestorSubtypeBindingClass).InnerClass.Value,
                ((AncestorSubtypeBindingClass) actual.AncestorSubtypeBindingClass).InnerClass.Value);
        }

        [Fact]
        public void SubtypeAsSourceTest()
        {
            var expected = new SubtypeAsSourceClass {Superclass = new SubclassA(), Name = "Alice"};
            var actual = Roundtrip(expected);
            Assert.Equal(expected.Name, actual.Name);
        }

        [Fact]
        public void IncompatibleBindingsTest()
        {
            var expected = new IncompatibleBindingsClass();

#if TESTASYNC
            Assert.Throws<AggregateException>(() => Roundtrip(expected));
#else
            Assert.Throws<InvalidOperationException>(() => Roundtrip(expected));
#endif
        }

        [Fact]
        public void InvalidSubtypeTest()
        {
            var expected = new InvalidSubtypeClass();
#if TESTASYNC
            Assert.Throws<AggregateException>(() => Roundtrip(expected));
#else
            Assert.Throws<InvalidOperationException>(() => Roundtrip(expected));
#endif
        }

        [Fact]
        public void NonUniqueSubtypesTest()
        {
            var expected = new NonUniqueSubtypesClass();
#if TESTASYNC
            Assert.Throws<AggregateException>(() => Roundtrip(expected));
#else
            Assert.Throws<InvalidOperationException>(() => Roundtrip(expected));
#endif
        }

        [Fact]
        public void NonUniqueSubtypeValuesTest()
        {
            var expected = new NonUniqueSubtypeValuesClass();

#if TESTASYNC
            Assert.Throws<AggregateException>(() => Roundtrip(expected));
#else
            Assert.Throws<InvalidOperationException>(() => Roundtrip(expected));
#endif
        }

        [Fact]
        public void DefaultSubtypeOnlyTest()
        {
            var actual = Deserialize<SubtypeDefaultOnlyClass>(new byte[] {0x4, 0x1, 0x2, 0x3, 0x4, 05});
            Assert.Equal(0x4, actual.Key);
            Assert.Equal(typeof(DefaultSubtypeClass), actual.Value.GetType());
        }

        [Fact]
        public void MultipleBindingModesTest()
        {
            var forward = new MixedBindingModesClass
            {
                Value = new SubclassB()
            };

            var actual = Roundtrip(forward, new byte[]{0x2, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0});
            Assert.Equal(typeof(SubclassA), actual.Value.GetType());
        }

        [Fact]
        public void UnorderedSubtypeTest()
        {
            var expected = new UnorderedSubtype();

#if TESTASYNC
            Assert.Throws<AggregateException>(() => Roundtrip(expected));
#else
            Assert.Throws<InvalidOperationException>(() => Roundtrip(expected));
#endif
        }

        [Fact]
        public void IgnoredSubtypeTest()
        {
            var expected = new IgnoredSubtype();
            Roundtrip(expected);
        }
    }
}