namespace BinarySerialization.Test.ItemSubtype;

public class ItemSubtypeFactory : ISubtypeFactory
{
    public bool TryGetKey(Type valueType, out object key)
    {
        if (valueType == typeof(ItemTypeA))
        {
            key = 1;
        }
        else if (valueType == typeof(ItemTypeB))
        {
            key = 3;
        }
        else
        {
            key = null;
            return false;
        }

        return true;
    }

    public bool TryGetType(object key, out Type type)
    {
        switch ((byte)key)
        {
            case 1:
                type = typeof(ItemTypeA);
                break;
            case 3:
                type = typeof(ItemTypeB);
                break;
            default:
                type = null;
                return false;
        }

        return true;
    }
}
