using System.Linq;
using System.Text;
using Xunit;

namespace BinarySerialization.Test.Encoding
{
    
    public class EncodingTests : TestBase
    {
        public EncodingTests()
        {
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [Fact]
        public void EncodingTest()
        {
            var expected = new EncodingClass {Name = "السلام عليكم"};
            var expectedData = System.Text.Encoding.GetEncoding("windows-1256").GetBytes(expected.Name + char.MinValue);
            var actual = Roundtrip(expected, expectedData);

            Assert.Equal(expected.Name, actual.Name);
        }

        [Fact]
        public void EncodingTest2()
        {
            var expected = new EncodingClass2 {Name = "السلام عليكم"};
            var expectedData = System.Text.Encoding.GetEncoding("windows-1256").GetBytes(expected.Name + char.MinValue);
            var actual = Roundtrip(expected, expectedData);

            Assert.Equal(expected.Name, actual.Name);
        }

        [Fact]
        public void FieldEncodingTest()
        {
            var expected = new FieldEncodingClass {Value = "السلام عليكم", Encoding = "windows-1256"};
            var encodingFieldData = System.Text.Encoding.UTF8.GetBytes(expected.Encoding + char.MinValue);
            var expectedValueData = System.Text.Encoding.GetEncoding(expected.Encoding).GetBytes(expected.Value + char.MinValue);

            var expectedData = encodingFieldData.Concat(expectedValueData).ToArray();

            var actual = Roundtrip(expected, expectedData);

            Assert.Equal(expected.Encoding, actual.Encoding);
            Assert.Equal(expected.Value, actual.Value);
        }

        [Fact]
        public void ConstFieldEncodingTest()
        {
            var expected = new ConstEncodingClass {Value = "السلام عليكم"};
            var expectedData = System.Text.Encoding.GetEncoding("windows-1256").GetBytes(expected.Value + char.MinValue);
            var actual = Roundtrip(expected, expectedData);

            Assert.Equal(expected.Value, actual.Value);
        }

        [Fact]
        public void NullTerminatedUtf16Test()
        {
            var expected = new EncodingClassUtf16 {Name = "hello"};
            var expectedData = System.Text.Encoding.Unicode.GetBytes(expected.Name + char.MinValue);
            var actual = Roundtrip(expected, expectedData);

            Assert.Equal(expected.Name, actual.Name);
        }
    }
}