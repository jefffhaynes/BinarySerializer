namespace BinarySerialization.Test.Issues.Issue147
{
    class ClassB
    {
        [FieldOrder(1)]
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once InconsistentNaming
        public int XXX;
        [FieldOrder(2)]
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once InconsistentNaming
        public int YYY;
    }
}