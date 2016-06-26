namespace BinarySerialization.Test.Issues.Issue30
{
    public interface IMessage<out T> where T : class, IPayload
    {
        Header Header { get; }
        T Payload { get; }
        void ComplementHeader();
    }
}
