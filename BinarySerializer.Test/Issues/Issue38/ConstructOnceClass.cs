namespace BinarySerialization.Test.Issues.Issue38
{
    public class ConstructOnceClass
    {
        public ConstructOnceClass()
        {
            Count++;
        }

        [Ignore]
        public static int Count { get; set; }

        public int OtherStuff { get; set; }
    }
}