
namespace Test_BinarySerialzer
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using BinarySerialization;

    using Zookeeper.Structs;

    class TestClass
    {
        [FieldOrder(0)]
        [SubtypeDefault(typeof(IPAddressEx))]
        public IIPAddressEx ipAddress { get; set; } = new IPAddressEx();

        [FieldOrder(1)]
        [SubtypeDefault(typeof(IPEndPointEx))]
        public IIPEndPointEx endPoint { get; set; } = new IPEndPointEx();
    }
}
