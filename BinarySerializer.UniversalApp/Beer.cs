using System.Collections.Generic;
using BinarySerialization;

namespace BinarySerializer.UniversalApp
{
    public class Beer
    {
        [SerializeAs(SerializedType.LengthPrefixedString)]
        [FieldOrder(0)]
        public string Brand;
        
        [FieldOrder(1)]
        public byte SortCount;

        [FieldOrder(2)]
        [FieldCount("SortCount")]
        public List<SortContainer> Sort;

        [FieldOrder(3)]
        public float Alcohol;

        [SerializeAs(SerializedType.LengthPrefixedString)]
        [FieldOrder(4)]
        public string Brewery;
    }
    
    public class SortContainer
    {
        [FieldOrder(0)]
        public byte NameLength;

        [FieldOrder(1)]
        [FieldLength("NameLength")]
        public string Name;
    }
}
