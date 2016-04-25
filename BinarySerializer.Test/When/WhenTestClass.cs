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

        [FieldOrder(4)]
        public int WhatToDo2 { get; set; }

        [FieldOrder(5)]
        [SerializeWhen("WhatToDo2", (byte)1)]
        public int SerializeThis2 { get; set; }

        [FieldOrder(6)]
        [SerializeWhen("WhatToDo2", (byte)2)]
        public int DontSerializeThis2 { get; set; }

        [FieldOrder(7)]
        [SerializeWhen("WhatToDo2", (byte)1)]
        [SerializeWhen("WhatToDo2", (byte)2)]
        public int SerializeThisNoMatterWhat2 { get; set; }
    }
}
