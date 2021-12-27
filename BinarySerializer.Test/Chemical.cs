namespace BinarySerialization.Test;

public abstract class Chemical
{
    protected Chemical(string formula)
    {
        Formula = formula;
    }

    [SerializeAs(SerializedType.TerminatedString)]
    public string Formula { get; set; }
}
