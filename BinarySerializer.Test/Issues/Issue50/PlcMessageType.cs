namespace BinarySerialization.Test.Issues.Issue50
{
    public enum PlcMessageType : ushort
    {
        Nok = 0,

        Ack = 1,

        Alive = 2,

        Data = 3,

        AckAlive = 4
    }
}