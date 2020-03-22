using System.IO;

namespace BinarySerialization.Test.Issues.Issue149
{
    public class TestContext

    {
        public string fileName;
        public uint version;
        public bool versionOK = false;
        public FileAccess fileAccessMode;
        public FileStream stream;
        public BinarySerialization.Endianness endianness = BinarySerialization.Endianness.Little;
        private bool _keepOpen;
        public int headerLength;
        public int numChannels;
        public bool compressed;
        public BinarySerializer serializer = new BinarySerializer();
    }
}
