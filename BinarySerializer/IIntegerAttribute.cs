namespace BinarySerialization
{
    internal interface IIntegerAttribute : IBindableFieldAttribute
    {
        ulong GetConstValue();
    }
}
