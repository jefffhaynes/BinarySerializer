using System;
using System.IO;
using System.Security.Cryptography;

namespace BinarySerialization.Converters
{
    public abstract class CrcBaseConverter : IValueConverter
    {
        protected abstract HashAlgorithm CrcAlgorithm { get; }

        public object Convert(object value, BinarySerializationContext ctx)
        {
            var serializer = new BinarySerializer();
            var stream = new MemoryStream();
            serializer.Serialize(stream, value);
            stream.Position = 0;
            return CrcAlgorithm.ComputeHash(stream);
        }

        public object ConvertBack(object value, BinarySerializationContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
