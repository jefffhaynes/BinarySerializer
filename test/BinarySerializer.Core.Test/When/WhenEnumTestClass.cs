namespace BinarySerialization.Test.When
{
    public class WhenEnumTestClass
    {
        [FieldOrder(8)]
        public int WhatToDo { get; set; }

        [FieldOrder(9)]
        [SerializeWhen("WhatToDo", WhenEnum.WhenOne)]
        public int SerializeThis { get; set; }

        [FieldOrder(10)]
        [SerializeWhen("WhatToDo", WhenEnum.WhenTwo)]
        public int DontSerializeThis { get; set; }

        [FieldOrder(11)]
        [SerializeWhen("WhatToDo", WhenEnum.WhenOne)]
        [SerializeWhen("WhatToDo", WhenEnum.WhenTwo)]
        public int SerializeThisNoMatterWhat { get; set; }
    }
}