using BinarySerialization.Test.Converters;

namespace BinarySerialization.Test.When
{
    public class WhenConverterTestClass
    {
        [FieldOrder(0)]
        public int WhatToDo { get; set; }

        [FieldOrder(1)]
        [SerializeWhen("WhatToDo", WhenEnum.WhenTwo, ConverterType = typeof(TwiceConverter))]
        public int SerializeThis { get; set; }

        [FieldOrder(2)]
        [SerializeWhen("WhatToDo", WhenEnum.WhenOne, ConverterType = typeof(TwiceConverter))]
        public int DontSerializeThis { get; set; }

        [FieldOrder(3)]
        [SerializeWhen("WhatToDo", WhenEnum.WhenOne, ConverterType = typeof(TwiceConverter))]
        [SerializeWhen("WhatToDo", WhenEnum.WhenTwo, ConverterType = typeof(TwiceConverter))]
        public int SerializeThisNoMatterWhat { get; set; }
    }
}