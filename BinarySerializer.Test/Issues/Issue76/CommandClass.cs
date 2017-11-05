namespace BinarySerialization.Test.Issues.Issue76
{
    public enum CommandClass
    {
        [SerializeAsEnum("sRN")]
        Srn,

        [SerializeAsEnum("sWN")]
        Swn,

        [SerializeAsEnum("sMN")]
        Smn,

        [SerializeAsEnum("sEN")]
        Sen,

        [SerializeAsEnum("sRA")]
        Sra,

        [SerializeAsEnum("sWA")]
        Swa,

        [SerializeAsEnum("sEA")]
        Sea,

        [SerializeAsEnum("sSN")]
        Ssn
    }

    public enum CommandType
    {
        SetAccessMode,

        [SerializeAsEnum("mLMPsetscancfg")]
        SetScanConfig,

        [SerializeAsEnum("LMDscandatacfg")]
        ScanDataConfig

        // etc
    }

    public class Packet
    {
        [FieldOrder(0)]
        public uint Length { get; set; }

        [FieldOrder(1)]
        [FieldLength("Length")]
        public PacketContent Content { get; set; }
    }

    public class PacketContent
    {
        [FieldOrder(0)]
        [SerializeAs(SerializedType.TerminatedString, StringTerminator = 0x20)]
        public CommandClass CommandClass { get; set; }

        [FieldOrder(1)]
        [Subtype("CommandClass", CommandClass.Smn, typeof(SmnCommandContainer))]
        public CommandContainer Payload { get; set; }
    }

    public abstract class CommandContainer
    {
        [SerializeAs(SerializedType.TerminatedString, StringTerminator = 0x20)]
        public CommandType CommandType { get; set; }
    }

    public class SmnCommandContainer : CommandContainer
    {
        [Subtype("CommandType", CommandType.SetAccessMode, typeof(SetAccessModeCommand))]
        public Command Command { get; set; }
    }

    public abstract class Command
    {
    }

    public enum UserLevel : byte
    {
        AuthorizedClient = 0x3
    }

    public class SetAccessModeCommand : Command
    {
        [FieldOrder(0)]
        public UserLevel UserLevel { get; set; } = UserLevel.AuthorizedClient;

        [FieldOrder(1)]
        [FieldLength(4)]
        public byte[] Password { get; set; } = { 0xF4, 0x72, 0x47, 0x44 };
    }
}
