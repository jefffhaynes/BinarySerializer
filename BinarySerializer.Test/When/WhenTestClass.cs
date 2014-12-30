using BinarySerialization;

namespace BinarySerializer.Test.When
{
    public class WhenTestClass
    {
        public string WhatToDo { get; set; }

        [SerializeWhen("WhatToDo", "PickOne")]
        public int SerializeThis { get; set; }

        [SerializeWhen("WhatToDo", "PickTwo")]
        public int DontSerializeThis { get; set; }

        [SerializeWhen("WhatToDo", "PickOne")]
        [SerializeWhen("WhatToDo", "PickTwo")]
        public int SerializeThisNoMatterWhat { get; set; }
    }
}
