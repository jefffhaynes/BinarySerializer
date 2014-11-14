using System.IO;
using System.Xml.Serialization;
using BinarySerialization;

namespace Iso9660test
{
    /// <summary>
    /// This example shows how the BinarySerializer can be used to deserialize a cd-rom "iso".  Streamlets are utilized to make
    /// the deserialization fairly lightweight and allow for deferred reading of data from the original source.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            var file = args[0];

            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                var serializer = new BinarySerializer();

                var iso = serializer.Deserialize<Iso9660.Iso9660>(stream);
                stream.Position = 0;

                var xmlSerializer = new XmlSerializer(typeof (Iso9660.Iso9660));

                var outfilePath = Path.GetDirectoryName(file);
                var outfileName = Path.GetFileNameWithoutExtension(file) + ".xml";
                var outfile = Path.Combine(outfilePath, outfileName);

                using (var xmlStream = new FileStream(outfile, FileMode.Create, FileAccess.Write))
                {
                    xmlSerializer.Serialize(xmlStream, iso);
                }
            }
        }
    }
}
