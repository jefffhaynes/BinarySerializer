namespace BinarySerializerTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new BinarySerializer.Test.BinarySerializerTests();

            for (int i = 0; i < 10000; i++)
            {
                test.Roundtrip();
            }
        }
    }
}
