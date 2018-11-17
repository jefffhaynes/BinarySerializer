namespace BinarySerialization.Test.Issues.Issue76
{
    public class SmnCommandContainer : CommandContainer
    {
        [Subtype("CommandType", CommandType.SetAccessMode, typeof(SetAccessModeCommand))]
        public Command Command { get; set; }
    }
}