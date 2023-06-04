namespace Zookeeper.Structs
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using BinarySerialization;

    /// <summary> Exception for signalling IP end point errors. </summary>
    /// <seealso cref="BinarySerialization.IBinarySerializable"/>
    public class IPEndPointEx : IBinarySerializable, IIPEndPointEx
    {
        /// <summary> Gets or sets the end point. </summary>
        /// <value> The end point. </value>
        [Ignore]
        public IPEndPoint EndPoint { get; set; } = new IPEndPoint(IPAddress.Loopback, 0);

        /// <summary> Deserialize this object to the given stream. </summary>
        /// <param name="stream">               The stream. </param>
        /// <param name="endianness">           The endianness. </param>
        /// <param name="serializationContext"> Context for the serialization. </param>
        public void Deserialize(Stream stream, Endianness endianness, BinarySerializationContext serializationContext)
        {
            var ip = new byte[4];
            var port = new byte[2];

            stream.Read(ip, 0, ip.Length);
            stream.Read(port, 0, port.Length);

            this.EndPoint.Address = new IPAddress(ip.Reverse().ToArray());
            this.EndPoint.Port = Convert.ToInt32(BitConverter.ToUInt16(port, 0));
        }

        /// <summary> Serialize this object to the given stream. </summary>
        /// <param name="stream">               The stream. </param>
        /// <param name="endianness">           The endianness. </param>
        /// <param name="serializationContext"> Context for the serialization. </param>
        public void Serialize(Stream stream, Endianness endianness, BinarySerializationContext serializationContext)
        {
            stream.Write(this.EndPoint.Address.GetAddressBytes().Reverse().ToArray(), 0, 4);
            stream.Write(BitConverter.GetBytes(Convert.ToUInt16(this.EndPoint.Port)), 0, 2);
        }
    }
}