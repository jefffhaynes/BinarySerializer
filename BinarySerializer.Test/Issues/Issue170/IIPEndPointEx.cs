namespace Zookeeper.Structs
{
    using System;
    using System.Linq;
    using System.Net;
    public interface IIPEndPointEx
    {
        /// <summary> Gets or sets the end point. </summary>
        /// <value> The end point. </value>
        IPEndPoint EndPoint { get; set; }
    }
}