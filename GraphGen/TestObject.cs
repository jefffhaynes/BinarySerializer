using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerialization;

namespace GraphGen
{
    class TestObject
    {
        public TestObject()
        {
            TestObject2 = new TestObject2();
            Items = new List<int>();
            StringArray = new[] {"hello", "world"};
        }

        public int NameLength { get; set; }

        [FieldLength("NameLength")]
        public string Name { get; set; }

        public uint Age { get; set; }

        public TestObject2 TestObject2 { get; set; }

        [FieldCount(3)]
        public List<int> Items;

        [FieldCount("Age")]
        public string[] StringArray { get; set; }

        //[Subtype("Age", 0, typeof(SubtypeChildClassA))]
        //[Subtype("Age", 1, typeof(SubtypeChildClassA))]
        //public SubtypeBaseClass Details { get; set; }
    }
}
