namespace BinarySerialization.Test.Issues.Issue76;

public class SetAccessModeCommand : Command
{
    [FieldOrder(0)]
    public UserLevel UserLevel { get; set; } = UserLevel.AuthorizedClient;

    [FieldOrder(1)]
    [FieldLength(4)]
    public byte[] Password { get; set; } = { 0xF4, 0x72, 0x47, 0x44 };
}
