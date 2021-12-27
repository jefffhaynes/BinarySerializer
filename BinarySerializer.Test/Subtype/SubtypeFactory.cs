namespace BinarySerialization.Test.Subtype;

public class SubtypeFactory : ISubtypeFactory
{
    public bool TryGetKey(Type valueType, out object key)
    {
        if (valueType == typeof(SubclassA))
        {
            key = 1;
        }
        else if (valueType == typeof(SubclassB))
        {
            key = 2;
        }
        else if (valueType == typeof(SubSubclassC))
        {
            key = 4;
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
        switch (Convert.ToInt32(key))
        {
            case 1:
                type = typeof(SubclassA);
                break;
            case 2:
                type = typeof(SubclassB);
                break;
            case 4:
                type = typeof(SubSubclassC);
                break;
            default:
                type = null;
                return false;
        }

        return true;
    }
}
