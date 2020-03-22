namespace BinarySerialization.Test.Issues.Issue147
{
    class ClassB
    {
        [FieldOrder(1)]
        public int XXX;
        [FieldOrder(2)]
        public int YYY;
    }
}