namespace BinarySerialization
{
    internal interface IConstAttribute : IBindableFieldAttribute
    {
        object GetConstValue();
    }
}
