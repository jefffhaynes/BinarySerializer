using System.Collections.Generic;

namespace BinarySerialization.Test.Issues.Issue147
{
    class Parent
    {
        [FieldOrder(1)] public ClassA config;

        [FieldOrder(2)]
        [FieldCount("config.count", RelativeSourceMode = RelativeSourceMode.FindAncestor,
            AncestorType = typeof(ClassA))]
        public List<ClassB> somelist;
    }
}
