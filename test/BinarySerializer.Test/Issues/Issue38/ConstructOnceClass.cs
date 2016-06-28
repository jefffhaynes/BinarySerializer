namespace BinarySerialization.Test.Issues.Issue38
{
    public class ConstructOnceClass
    {
        [Ignore]
        public static int Count { get; set; }

        public ConstructOnceClass()
        {
            Count++;
        }

        public int OtherStuff { get; set; }
    }
}
