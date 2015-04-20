namespace BinarySerialization.Test.Issues.Issue12
{
    public enum ChunkType
    {
        [SerializeAsEnum("FORM")]
        Form,

        [SerializeAsEnum("CAT ")]
        Cat,

        [SerializeAsEnum("LIST")]
        List,

        [SerializeAsEnum("REFE")]
        Refe
    }
}
