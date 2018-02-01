namespace BinarySerialization.Test.Issues.Issue90
{
    public abstract class TxpFile
    {
        [FieldOrder(0)] public char[] Magic = { 'T', 'X', 'P' };
        [FieldOrder(1)]
        [Subtype("Magic", "TXP", typeof(TxpTexture))]
        [Subtype("Magic", "TXP", typeof(TxpTextureAtlas))]
        public TxpBase txp;
    }
}