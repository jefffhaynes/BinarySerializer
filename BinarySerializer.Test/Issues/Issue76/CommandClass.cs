namespace BinarySerialization.Test.Issues.Issue76;

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
