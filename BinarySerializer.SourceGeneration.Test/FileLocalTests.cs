using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.SourceGeneration.Test
{
    /// <summary>
    ///     C# 11 file-local types cannot be referenced from the generated registry file.  The
    ///     generator must skip them (no generated registration) and the serialization path must fall
    ///     back to reflection.  The file-local type lives in this file because it is only visible here.
    /// </summary>
    [TestClass]
    public class FileLocalTests
    {
        [TestMethod]
        public void FileLocalTypeIsNotRegistered()
        {
            /* No generated accessors or factory: the generated class cannot see a file-local type. */
            Assert.IsFalse(GeneratedTypeRegistry.TryGetAccessors(typeof(FileLocalPacket), "Z", out _, out _));
            Assert.IsFalse(GeneratedTypeRegistry.TryGetParameterlessConstructor(typeof(FileLocalPacket), out _));
        }

        [TestMethod]
        public void FileLocalTypeFallsBackToReflection()
        {
            /* Serialization must still work through the reflection discovery path. */
            var serializer = new BinarySerializer();
            var expected = new FileLocalPacket {Z = 7};

            byte[] data;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, expected);
                data = stream.ToArray();
            }

            CollectionAssert.AreEqual(new byte[] {7, 0, 0, 0}, data);

            var deserialized = (FileLocalPacket) serializer.Deserialize(data, typeof(FileLocalPacket));
            Assert.AreEqual(7, deserialized.Z);
        }
    }

    [BinarySerializerRoot]
    file class FileLocalPacket
    {
        [FieldOrder(0)]
        public int Z { get; set; }
    }
}
