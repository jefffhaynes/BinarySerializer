namespace BinarySerialization.Test.Issues.Issue90;

public abstract class TxpBase
{
    [FieldCount(4), FieldLength(4)] public string Magic = "TXP0";
}
