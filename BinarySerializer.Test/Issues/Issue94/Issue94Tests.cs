#pragma warning disable 649

namespace BinarySerialization.Test.Issues.Issue94;

[TestClass]
public class Issue94Tests : TestBase
{
    class MainClass
    {
        [FieldOrder(0)] public int Count; // 3
        [FieldOrder(1)] public int Offset; // 15
        [FieldOrder(2), FieldCount("Count"), FieldOffset("Offset")] public List<Entries> Entries;
    }

    class Entries
    {
        [FieldOrder(0)] public int id;
        [FieldOrder(1)] public int Offset;
        [FieldOrder(2), FieldEncoding("ASCII"), FieldOffset(0)] public string String;
    }

    [TestMethod]
    public void Test()
    {
        using var file = new FileStream(@"Issues\Issue94\tst.file", FileMode.Open, FileAccess.Read);
        var db = Deserialize<MainClass>(file);
    }
}
