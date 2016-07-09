using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerialization.Test.Length
{
    public class MultibindingClass
    {
        [FieldOrder(0)]
        public byte Length { get; set; }
        
        [FieldOrder(1)]
        public byte Length2 { get; set; }

        [FieldOrder(2)]
        [FieldLength("Length")]
        [FieldLength("Length2")]
        public string Value { get; set; }
    }
}
