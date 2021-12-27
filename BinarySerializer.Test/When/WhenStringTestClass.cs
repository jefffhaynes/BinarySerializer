namespace BinarySerialization.Test.When;

public class WhenStringTestClass
{
    [FieldOrder(0)]
    public string WhatToDo { get; set; }

    [FieldOrder(1)]
    [SerializeWhen(nameof(WhatToDo), "PickOne")]
    public int SerializeThis { get; set; }

    [FieldOrder(2)]
    [SerializeWhen(nameof(WhatToDo), "PickTwo")]
    public int DontSerializeThis { get; set; }

    [FieldOrder(3)]
    [SerializeWhen(nameof(WhatToDo), "PickOne")]
    [SerializeWhen(nameof(WhatToDo), "PickTwo")]
    public int SerializeThisNoMatterWhat { get; set; }
}
