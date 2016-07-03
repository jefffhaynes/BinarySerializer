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
            var expected = new SubtypeClass {Field = new SubclassB{SomethingForClassB = 33}, Field2 = new SubclassA()};
            var actual = Roundtrip(expected);

            Assert.Equal(SubclassType.B, actual.Subtype);
            Assert.IsAssignableFrom(typeof(SubclassB),actual.Field);
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
            Assert.IsAssignableFrom(typeof(SubSubclassC),actual.Field);
            Assert.Equal(actual.Field.SomeSuperStuff, expected.Field.SomeSuperStuff);
            Assert.Equal(((SubSubclassC)actual.Field).SomethingForClassB, ((SubSubclassC)expected.Field).SomethingForClassB);
            Assert.Equal(((SubSubclassC)actual.Field).SomethingForClassC, ((SubSubclassC)expected.Field).SomethingForClassC);
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

            Assert.Equal(typeof(SubclassA), actualItems[0].Value.GetType());
            Assert.Null(actualItems[1].Value);
            Assert.Equal(typeof(SubSubclassC), actualItems[2].Value.GetType());
        }

        //[TestMethod]
        //[ExpectedException(typeof(BindingException))]
        //public void MissingSubtypeTest()
        //{
        //    var expected = new IncompleteSubtypeClass { Field = new SubclassB() };
        //    Roundtrip(expected);
        //}

        //[TestMethod]
        //public void BestFitSubtypeTest()
        //{
        //    var expected = new SubtypeClass { Field = new UnspecifiedSubclass() };
        //    var actual = Roundtrip(expected);

        //    Assert.AreEqual(SubclassType.B, actual.Subtype);
        //    Assert.IsInstanceOfType(actual.Field, typeof(SubclassB));
        //}

        [Fact]
        public void AncestorSubtypeBindingTest()
        {
            var expected = new AncestorSubtypeBindingContainerClass
            {
                AncestorSubtypeBindingClass =
                    new AncestorSubtypeBindingClass
                    {
                        InnerClass = new AncestorSubtypeBindingInnerClass { Value = "hello" }
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
            var e = Record.Exception(() => Roundtrip(expected));
            Assert.NotNull(e);
            Assert.IsType<InvalidOperationException>(e);
        }

        [Fact]
        //[ExpectedException(typeof(InvalidOperationException))]
        public void InvalidSubtypeTest()
        {
            var expected = new InvalidSubtypeClass();
            var e = Record.Exception(() => Roundtrip(expected));
            Assert.NotNull(e);
            Assert.IsType<InvalidOperationException>(e);

        }

        [Fact]
        public void NonUniqueSubtypesTest()
        {
            var expected = new NonUniqueSubtypesClass();
            var e = Record.Exception(() => Roundtrip(expected));
            Assert.NotNull(e);
            Assert.IsType<InvalidOperationException>(e);
        }

        [Fact]
        public void NonUniqueSubtypeValuesTest()
        {
            var expected = new NonUniqueSubtypeValuesClass();
            var e = Record.Exception(() => Roundtrip(expected));
            Assert.NotNull(e);
            Assert.IsType<InvalidOperationException>(e);

        }

        [Fact]
       // [ExpectedException(typeof (InvalidOperationException))]
        public void ThrowOnAbstractTypeWithNoSubtypeTest()
        {
            var e = Record.Exception(() => Roundtrip(new ThrowOnAbstractTypeWithNoSubtypeClass { Superclass = new SubclassA() }));
            Assert.NotNull(e);
            Assert.IsType<InvalidOperationException>(e);
        }
    }
}
