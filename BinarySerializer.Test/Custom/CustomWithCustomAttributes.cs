using System.IO;
using System.Linq;
using System.Reflection;
using BinarySerialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Custom
{
    public class CustomWithCustomAttributes : IBinarySerializable
    {
        public void Serialize(Stream stream, Endianness endianness, BinarySerializationContext serializationContext)
        {
            var memberInfo = serializationContext.MemberInfo;
            AssertCustomAttribute(memberInfo);
        }

        public void Deserialize(Stream stream, Endianness endianness, BinarySerializationContext serializationContext)
        {
            var memberInfo = serializationContext.MemberInfo;
            AssertCustomAttribute(memberInfo);
        }

        private void AssertCustomAttribute(MemberInfo memberInfo)
        {
            var attributes = memberInfo.CustomAttributes;
            var customAttribute = attributes.Single();
            Assert.AreEqual(typeof(CustomAttribute), customAttribute.AttributeType);
        }
    }
}
