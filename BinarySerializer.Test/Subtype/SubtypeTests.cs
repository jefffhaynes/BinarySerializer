using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Subtype
{
    [TestClass]
    public class SubtypeTests : TestBase
    {
        [TestMethod]
        public void SubtypeTest()
        {
            var expected = new SubtypeClass {Field = new SubclassB {SomethingForClassB = 33}, Field2 = new SubclassA()};
            var actual = Roundtrip(expected);

            Assert.AreEqual(SubclassType.B, actual.Subtype);
            Assert.IsTrue(actual.Field is SubclassB);
        }

        [TestMethod]
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

            Assert.AreEqual(SubclassType.C, actual.Subtype);
            Assert.IsTrue(actual.Field is SubSubclassC);
            Assert.AreEqual(actual.Field.SomeSuperStuff, expected.Field.SomeSuperStuff);
            Assert.AreEqual(((SubSubclassC) actual.Field).SomethingForClassB,
                ((SubSubclassC) expected.Field).SomethingForClassB);
            Assert.AreEqual(((SubSubclassC) actual.Field).SomethingForClassC,
                ((SubSubclassC) expected.Field).SomethingForClassC);
        }

        [TestMethod]
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

            Assert.AreEqual(typeof (SubclassA), actualItems[0].Value.GetType());
            Assert.IsNull(actualItems[1].Value);
            Assert.AreEqual(typeof (SubSubclassC), actualItems[2].Value.GetType());
        }

        [TestMethod]
        public void MissingSubtypeTest()
        {
            var expected = new IncompleteSubtypeClass { Field = new SubclassB() };
#if TESTASYNC
            Assert.ThrowsException<AggregateException>(() => Roundtrip(expected));
#else
            Assert.ThrowsException<InvalidOperationException>(() => Roundtrip(expected));
#endif
        }

        [TestMethod]
        public void SubtypeDefaultTest()
        {
            var data = new byte[] {0x0, 0x1, 0x2, 0x3, 0x4, 0x5};
            var actual = Deserialize<DefaultSubtypeContainerClass>(data);
            Assert.AreEqual(typeof(DefaultSubtypeClass), actual.Value.GetType());
        }

        [TestMethod]
        public void InvalidSubtypeDefaultTest()
        {
            var data = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };
#if TESTASYNC
            Assert.ThrowsException<AggregateException>(() =>
#else
            Assert.ThrowsException<InvalidOperationException>(() =>
#endif
            Deserialize<InvalidDefaultSubtypeContainerClass>(data));

        }

        [TestMethod]
        public void DefaultSubtypeForwardTest()
        {
            var expected = new DefaultSubtypeContainerClass
            {
                Value = new SubclassA()
            };

            var actual = Roundtrip(expected);

            Assert.AreEqual(1, actual.Indicator);
            Assert.AreEqual(typeof(SubclassA), actual.Value.GetType());
        }

        [TestMethod]
        public void DefaultSubtypeAllowOnSerialize()
        {
            var expected = new DefaultSubtypeContainerClass
            {
                Indicator = 33,
                Value = new DefaultSubtypeClass()
            };

            var actual = Roundtrip(expected);

            Assert.AreEqual(33, actual.Indicator);
            Assert.AreEqual(typeof(DefaultSubtypeClass), actual.Value.GetType());
        }

        [TestMethod]
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
            Assert.AreEqual(((AncestorSubtypeBindingClass) expected.AncestorSubtypeBindingClass).InnerClass.Value,
                ((AncestorSubtypeBindingClass) actual.AncestorSubtypeBindingClass).InnerClass.Value);
        }

        [TestMethod]
        public void SubtypeAsSourceTest()
        {
            var expected = new SubtypeAsSourceClass {Superclass = new SubclassA(), Name = "Alice"};
            var actual = Roundtrip(expected);
            Assert.AreEqual(expected.Name, actual.Name);
        }

        [TestMethod]
        public void IncompatibleBindingsTest()
        {
            var expected = new IncompatibleBindingsClass();

#if TESTASYNC
            Assert.ThrowsException<AggregateException>(() => Roundtrip(expected));
#else
            Assert.ThrowsException<InvalidOperationException>(() => Roundtrip(expected));
#endif
        }

        [TestMethod]
        public void InvalidSubtypeTest()
        {
            var expected = new InvalidSubtypeClass();
#if TESTASYNC
            Assert.ThrowsException<AggregateException>(() => Roundtrip(expected));
#else
            Assert.ThrowsException<InvalidOperationException>(() => Roundtrip(expected));
#endif
        }

        [TestMethod]
        public void NonUniqueSubtypesTest()
        {
            var expected = new NonUniqueSubtypesClass();
#if TESTASYNC
            Assert.ThrowsException<AggregateException>(() => Roundtrip(expected));
#else
            Assert.ThrowsException<InvalidOperationException>(() => Roundtrip(expected));
#endif
        }

        [TestMethod]
        public void NonUniqueSubtypeValuesTest()
        {
            var expected = new NonUniqueSubtypeValuesClass();

#if TESTASYNC
            Assert.ThrowsException<AggregateException>(() => Roundtrip(expected));
#else
            Assert.ThrowsException<InvalidOperationException>(() => Roundtrip(expected));
#endif
        }

        [TestMethod]
        public void DefaultSubtypeOnlyTest()
        {
            var actual = Deserialize<SubtypeDefaultOnlyClass>(new byte[] {0x4, 0x1, 0x2, 0x3, 0x4, 05});
            Assert.AreEqual(0x4, actual.Key);
            Assert.AreEqual(typeof(DefaultSubtypeClass), actual.Value.GetType());
        }

        [TestMethod]
        public void MultipleBindingModesTest()
        {
            var forward = new MixedBindingModesClass
            {
                Value = new SubclassB()
            };

            var actual = Roundtrip(forward, new byte[]{0x2, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0});
            Assert.AreEqual(typeof(SubclassA), actual.Value.GetType());
        }

        [TestMethod]
        public void UnorderedSubtypeTest()
        {
            var expected = new SuperclassContainerWithNoBinding
            {
                Superclass = new UnorderedSubtype()
            };

#if TESTASYNC
            Assert.ThrowsException<AggregateException>(() => Roundtrip(expected));
#else
            Assert.ThrowsException<InvalidOperationException>(() => Roundtrip(expected));
#endif
        }

        [TestMethod]
        public void IgnoredSubtypeTest()
        {
            var expected = new SuperclassContainerWithNoBinding
            {
                Superclass = new IgnoredSubtype()
            };

            Roundtrip(expected);
        }
    }
}