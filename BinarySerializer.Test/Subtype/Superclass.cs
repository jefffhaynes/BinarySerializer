namespace BinarySerialization.Test.Subtype
{
    public interface ISuperclass
    {
        public int SomeSuperStuff { get; set; }
    }

    public abstract class Superclass : ISuperclass
    {
        public int SomeSuperStuff { get; set; }
    }
}