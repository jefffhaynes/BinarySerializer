using System.Linq;
using System.Text;
using Xunit;

namespace BinarySerialization.Test.Encoding
{
    
    public class EncodingTests : TestBase
    {
        //[TestInitialize]
        //public void Initialize()
        //{
        //    System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //}
        public EncodingTests()
        {
                System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [Fact]
        public void EncodingTest()
        {
            var expected = new EncodingClass {Name = "السلام عليكم" };
            var expectedData = System.Text.Encoding.GetEncoding("windows-1256").GetBytes(expected.Name + "\0");
            var actual = Roundtrip(expected, expectedData);

            Assert.Equal(expected.Name, actual.Name);
        }

        [Fact]
        public void FieldEncodingTest()
        {
            var expected = new FieldEncodingClass {Value = "السلام عليكم", Encoding = "windows-1256"};
            var encodingFieldData = System.Text.Encoding.UTF8.GetBytes(expected.Encoding + "\0");
            var expectedValueData = System.Text.Encoding.GetEncoding(expected.Encoding).GetBytes(expected.Value + "\0");

            var expectedData = encodingFieldData.Concat(expectedValueData).ToArray();

            var actual = Roundtrip(expected, expectedData);

            Assert.Equal(expected.Encoding, actual.Encoding);
            Assert.Equal(expected.Value, actual.Value);
        }

        [Fact]
        public void ConstFieldEncodingTest()
        {
            var expected = new ConstEncodingClass {Value = "السلام عليكم"};
            var expectedData = System.Text.Encoding.GetEncoding("windows-1256").GetBytes(expected.Value + "\0");
            var actual = Roundtrip(expected, expectedData);

            Assert.Equal(expected.Value, actual.Value);
        }
    }
}