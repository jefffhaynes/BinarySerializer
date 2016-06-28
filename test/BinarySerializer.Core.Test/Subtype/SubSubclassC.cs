namespace BinarySerialization.Test.Subtype
{
    public class SubSubclassC : SubclassB
    {
        public SubSubclassC(double somethingForClassC)
        {
            SomethingForClassC = somethingForClassC;
        }

        public double SomethingForClassC { get; set; }
    }
}