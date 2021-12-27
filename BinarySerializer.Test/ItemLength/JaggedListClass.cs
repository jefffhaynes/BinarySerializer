﻿namespace BinarySerialization.Test.ItemLength;

public class JaggedListClass
{
    [FieldOrder(0)]
    public int NameCount { get; set; }

    [FieldOrder(1)]
    [FieldCount(nameof(NameCount))]
    public int[] NameLengths { get; set; }

    [FieldOrder(2)]
    [FieldCount(nameof(NameCount))]
    [ItemLength(nameof(NameLengths))]
    public List<string> NameList { get; set; }
}
