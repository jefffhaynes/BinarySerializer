namespace Zookeeper.Structs
{
    using System;
    using System.Linq;
    using System.Net;

    /// <summary> Interface for iip address exception. </summary>
    public interface IIPAddressEx
    {
        /// <summary> Gets or sets the end point. </summary>
        /// <value> The end point. </value>
        IPAddress Address { get; set; }
    }
}