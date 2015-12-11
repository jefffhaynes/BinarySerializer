using BinarySerialization;

namespace BinarySerialization.Test.When
{
    public class WhenTestClass
    {
        [FieldOrder(0)]
        public string WhatToDo { get; set; }

        [FieldOrder(1)]
        [SerializeWhen("WhatToDo", "PickOne")]
        public int SerializeThis { get; set; }

        [FieldOrder(2)]
        [SerializeWhen("WhatToDo", "PickTwo")]
        public int DontSerializeThis { get; set; }

        [FieldOrder(3)]
        [SerializeWhen("WhatToDo", "PickOne")]
        [SerializeWhen("WhatToDo", "PickTwo")]
        public int SerializeThisNoMatterWhat { get; set; }
    }
}
