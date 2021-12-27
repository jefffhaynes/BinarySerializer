namespace BinarySerialization.Test.Issues.Issue140;

public interface IDomain
{
    List<ILabel> Labels { get; }
    string Name { get; }
}
