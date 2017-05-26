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
            Assert.IsInstanceOfType(actual.Field, typeof (SubclassB));
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
            Assert.IsInstanceOfType(actual.Field, typeof (SubSubclassC));
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void MissingSubtypeTest()
        {
            var expected = new IncompleteSubtypeClass { Field = new SubclassB() };
            Roundtrip(expected);
        }

        [TestMethod]
        public void SubtypeDefaultTest()
        {
            var data = new byte[] {0x0, 0x1, 0x2, 0x3, 0x4, 0x5};
            var actual = Deserialize<DefaultSubtypeContainerClass>(data);
            Assert.AreEqual(typeof(DefaultSubtypeClass), actual.Value.GetType());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InvalidSubtypeDefaultTest()
        {
            var data = new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5 };
            Deserialize<InvalidDefaultSubtypeContainerClass>(data);
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
        [ExpectedException(typeof (InvalidOperationException))]
        public void IncompatibleBindingsTest()
        {
            var expected = new IncompatibleBindingsClass();
            Roundtrip(expected);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void InvalidSubtypeTest()
        {
            var expected = new InvalidSubtypeClass();
            Roundtrip(expected);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void NonUniqueSubtypesTest()
        {
            var expected = new NonUniqueSubtypesClass();
            Roundtrip(expected);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void NonUniqueSubtypeValuesTest()
        {
            var expected = new NonUniqueSubtypeValuesClass();
            Roundtrip(expected);
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
            var forward = new MultipleBindingModesClass
            {
                Value = new SubclassB()
            };

            var actual = Roundtrip(forward);
            Assert.AreEqual(typeof(SubclassA), actual.Value.GetType());
        }
    }
}