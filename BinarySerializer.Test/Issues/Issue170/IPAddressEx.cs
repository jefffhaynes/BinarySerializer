namespace Zookeeper.Structs
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using BinarySerialization;

    /// <summary> Exception for signalling IP end point errors. </summary>
    /// <seealso cref="BinarySerialization.IBinarySerializable"/>
    public class IPAddressEx : IBinarySerializable, IIPAddressEx
    {
        /// <summary> Gets or sets the end point. </summary>
        /// <value> The end point. </value>
        [Ignore]
        public IPAddress Address { get; set; } = IPAddress.Loopback;

        /// <summary> Deserialize this object to the given stream. </summary>
        /// <param name="stream">               The stream. </param>
        /// <param name="endianness">           The endianness. </param>
        /// <param name="serializationContext"> Context for the serialization. </param>
        public void Deserialize(Stream stream, Endianness endianness, BinarySerializationContext serializationContext)
        {
            var ip = new byte[4];

            stream.Read(ip, 0, ip.Length);

            this.Address = new IPAddress(ip.Reverse().ToArray());
        }

        /// <summary> Serialize this object to the given stream. </summary>
        /// <param name="stream">               The stream. </param>
        /// <param name="endianness">           The endianness. </param>
        /// <param name="serializationContext"> Context for the serialization. </param>
        public void Serialize(Stream stream, Endianness endianness, BinarySerializationContext serializationContext)
        {
            stream.Write(this.Address.GetAddressBytes().Reverse().ToArray(), 0, 4);
        }
    }
}