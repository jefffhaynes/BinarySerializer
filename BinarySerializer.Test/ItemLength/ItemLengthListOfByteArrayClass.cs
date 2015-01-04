using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerialization;

namespace BinarySerializer.Test.ItemLength
{
    public class ItemLengthListOfByteArrayClass
    {
        [ItemLength(3)]
        public List<byte[]> Arrays { get; set; } 
    }
}
