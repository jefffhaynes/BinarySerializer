namespace BinarySerialization.Test.Custom;

public class CustomIListClass : IList<string>, IBinarySerializable
{
    private readonly List<string> _list = new();

    public IEnumerator<string> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    public void Add(string item)
    {
        _list.Add(item);
    }

    public void Clear()
    {
        _list.Clear();
    }

    public bool Contains(string item)
    {
        return _list.Contains(item);
    }

    public void CopyTo(string[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    public bool Remove(string item)
    {
        return _list.Remove(item);
    }

    [Ignore]
    public int Count => _list.Count;

    [Ignore]
    public bool IsReadOnly => false;

    public int IndexOf(string item)
    {
        return _list.IndexOf(item);
    }

    public void Insert(int index, string item)
    {
        _list.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _list.RemoveAt(index);
    }

    [Ignore]
    public string this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    public void Serialize(Stream stream, BinarySerialization.Endianness endianness, BinarySerializationContext serializationContext)
    {
        foreach (var item in this)
        {
            var data = System.Text.Encoding.UTF8.GetBytes(item);
            stream.WriteByte((byte)data.Length);
            stream.Write(data, 0, data.Length);
        }
    }

    public void Deserialize(Stream stream, BinarySerialization.Endianness endianness, BinarySerializationContext serializationContext)
    {
        while (stream.Position < stream.Length)
        {
            var length = stream.ReadByte();
            var data = new byte[length];
            stream.Read(data, 0, data.Length);
            var item = System.Text.Encoding.UTF8.GetString(data);
            Add(item);
        }
    }
}
