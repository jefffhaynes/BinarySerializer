using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerialization.Test.Issues.Issue18
{
    public class ArrayIndexerClass
    {
        public int[] ArrayLengths { get; set; }

        [ItemLength("ArrayLengths")]
        public byte[][] Arrays { get; set; }
    }
}
