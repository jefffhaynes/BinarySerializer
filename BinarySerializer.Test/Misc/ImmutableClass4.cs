using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerialization.Test.Misc
{
    public class ImmutableClass4
    {
        [FieldOrder(0)]
        public int Header { get; private set; }

        [Ignore]
        public int? ResponseId { get; private set; }

        public ImmutableClass4(int header, int? responseId = null)
        {
            Header = header;
            ResponseId = responseId;
        }

        public ImmutableClass4(int header)
        {
            Header = header;
        }
    }
}
