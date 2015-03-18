using System;
using System.Collections.Generic;

namespace BinarySerialization.Graph.TypeGraph
{
    public class EnumInfo
    {
        public Type UnderlyingType { get; set; }

        public SerializedType SerializedType { get; set; }

        public IDictionary<Enum, string> EnumValues { get; set; }

        public IDictionary<string, Enum> ValueEnums { get; set; }

        public int? EnumValueLength { get; set; }
    }
}
