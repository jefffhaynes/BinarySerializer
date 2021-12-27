namespace BinarySerialization.Test.Issues.Issue76;

public enum CommandType
{
    SetAccessMode,

    [SerializeAsEnum("mLMPsetscancfg")]
    SetScanConfig,

    [SerializeAsEnum("LMDscandatacfg")]
    ScanDataConfig

    // etc
}
