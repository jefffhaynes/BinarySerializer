namespace BinarySerialization.Test.Issues.Issue90;

public class TxpTextureAtlas : TxpBase
{
    public TxpTextureAtlas() => Magic = "TXP"; //TXP3
    [FieldOrder(1)] public int TextureCount;
    [FieldOrder(2)] public uint Version = 0x1010112;
    [FieldOrder(3), FieldCount("TextureCount")] public List<int> OffsetTable = new();
    [FieldOrder(4), FieldCount("TextureCount")] public List<TxpTexture> Textures = new();

    //public void SetTextureOffsets()
    //{
    //    OffsetTable = new List<int>(TextureCount);
    //    var offset = 12 + TextureCount * 4;
    //    for (var counter = 0; counter < Textures.Count; ++counter)
    //    {
    //        offset += counter != 0 ? Textures[counter - 1].Size : 0;
    //        OffsetTable.Add(offset);
    //    }
    //}

    public void SetTextures(Stream file)
    {
        var serializer = new BinarySerialization.BinarySerializer();
        Textures = new List<TxpTexture>(TextureCount);
        file.Seek(OffsetTable[0], SeekOrigin.Begin);
        OffsetTable.ForEach(offset => Textures.Add(serializer.Deserialize<TxpTexture>(file)));
    }


    public override string ToString() => $"TXP3: {TextureCount} textures, ({Textures.Count} serialized)";
}
