namespace BinarySerialization;

/// <summary>
/// Used to denote the type of a subtype factory object that implements ISubtypeFactory.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class ItemSubtypeFactoryAttribute : SubtypeFactoryBaseAttribute
{
    public ItemSubtypeFactoryAttribute(string path, Type factoryType) : base(path, factoryType)
    {
    }
}
