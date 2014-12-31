using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinarySerialization.Graph
{
    public class EnumInfo
    {
        public SerializedType SerializedType { get; set; }

        public IDictionary<Enum, string> EnumValues { get; set; }
        public IDictionary<string, Enum> ValueEnums { get; set; }

        public Type UnderlyingType { get; set; }
        public int? EnumValueLength { get; set; }
    }
}
