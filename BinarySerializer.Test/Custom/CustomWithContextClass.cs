using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Custom
{
    public class CustomWithContextClass : IBinarySerializable
    {
        public void Serialize(Stream stream, BinarySerialization.Endianness endianness,
            BinarySerializationContext serializationContext)
        {
            Assert.AreEqual(typeof(CustomWithContextContainerClass), serializationContext.ParentType);


            Assert.AreEqual(typeof(CustomWithContextContainerContextClass), serializationContext.ParentContext.ParentContext.ParentType);
            Assert.AreEqual("context", (serializationContext.ParentContext.ParentContext.ParentValue as CustomWithContextContainerContextClass).Value);
        }

        public void Deserialize(Stream stream, BinarySerialization.Endianness endianness,
            BinarySerializationContext serializationContext)
        {
            Assert.AreEqual(typeof(CustomWithContextContainerClass), serializationContext.ParentType);
        }
    }
}