using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerialization;

namespace BinarySerialization.Test.UntilItem
{
    public class UntilItemClass
    {
        [FieldOrder(0)]
        public string Name { get; set; }

        [FieldOrder(1)]
        public string LastItem { get; set; }

        [FieldOrder(2)]
        public string Description { get; set; }
    }
}
