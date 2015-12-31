using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Custom
{
    [TestClass]
    public class CustomTests : TestBase
    {
        [TestMethod]
        public void TestVaruint()
        {
            var expected = new Varuint {Value = ushort.MaxValue};
            var actual = Roundtrip(expected, 3);

            Assert.AreEqual(expected.Value, actual.Value);
        }

        [TestMethod]
        public void CustomWithContextTest()
        {
            var expected = new CustomWithContextClass();

            var serializer = new BinarySerializer();
            var stream = new MemoryStream();

            serializer.Serialize(stream, expected, "context");
        }

        [TestMethod]
        public void CustomSourceBindingTest()
        {
            var expected = new CustomSourceBinding {NameLength = new Varuint(), Name = "Alice"};
            var nameLength = System.Text.Encoding.UTF8.GetByteCount(expected.Name);
            var actual = Roundtrip(expected, nameLength + 1);
            Assert.AreEqual(expected.Name, actual.Name);
        }

        [TestMethod]
        public void CustomSourceBindingTest2()
        {
            var expected = new CustomSourceBinding { NameLength = new Varuint(), Name = "This is rather as if you imagine a puddle waking up one morning and thinking, 'This is an interesting world I find myself in — an interesting hole I find myself in — fits me rather neatly, doesn't it? In fact it fits me staggeringly well, must have been made to have me in it!' This is such a powerful idea that as the sun rises in the sky and the air heats up and as, gradually, the puddle gets smaller and smaller, frantically hanging on to the notion that everything's going to be alright, because this world was meant to have him in it, was built to have him in it; so the moment he disappears catches him rather by surprise. I think this may be something we need to be on the watch out for." };
            var nameLength = System.Text.Encoding.UTF8.GetByteCount(expected.Name);
            var actual = Roundtrip(expected, nameLength + 2);
            Assert.AreEqual(expected.Name, actual.Name);
        }

        [TestMethod]
        public void CustomSubtypeTest()
        {
            var expected = new CustomSubtypeContainerClass
            {
                Inner = new CustomSubtypeCustomClass
                {
                    Value = 2097151
                }
            };

            var actual = Roundtrip(expected, 150);

            var innerExpected = (CustomSubtypeCustomClass)expected.Inner;
            var innerActual = (CustomSubtypeCustomClass) actual.Inner;

            Assert.AreEqual(innerExpected.Value, innerActual.Value);
        }
    }
}
