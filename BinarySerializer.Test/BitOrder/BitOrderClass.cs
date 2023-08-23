using BinarySerialization;

namespace BinarySerialization.Test.FieldBitOrder
{
    public class BitOrderClass
    {
        [FieldOrder(0)]
        [FieldBitLength(4)]
        [FieldBitOrder(BitOrder.MsbFirst)]
        public byte Value1 { get; set; }

        [FieldOrder(1)]
        [FieldBitLength(4)]
        [FieldBitOrder(BitOrder.MsbFirst)]
        public byte Value2 { get; set; }
    }

    public class CipMessageRouterDataForward
    {
        [FieldOrder(0)]
        [FieldBitLength(1)]
        [FieldBitOrder(BitOrder.MsbFirst)]
        public bool Response = false;

        [FieldOrder(1)]
        [FieldBitLength(7)]
        [FieldBitOrder(BitOrder.MsbFirst)]
        public CipServiceCodes Service;
    }

    public class CipMessageRouterDataBackward
    {
        [FieldOrder(0)]
        [FieldBitLength(7)]
        public CipServiceCodes Service;

        [FieldOrder(1)]
        [FieldBitLength(1)]
        public bool Response = false;
    }

    public enum CipServiceCodes : byte
    {
        GetAttributesAll = 0x01,
        SetAttributesAllRequest = 0x02,
        GetAttributeList = 0x03,
        GetAttributeSingle = 0x0E,
    }

}
