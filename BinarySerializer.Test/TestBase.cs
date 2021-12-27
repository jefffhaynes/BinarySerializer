namespace BinarySerialization.Test;

public abstract class TestBase
{
    protected static readonly BinarySerializer Serializer = new BinarySerializer();

    protected static readonly BinarySerializer SerializerBe = new BinarySerializer
    {
        Endianness = BinarySerialization.Endianness.Big
    };

    protected static readonly string[] TestSequence = { "a", "b", "c" };
    protected static readonly int[] PrimitiveTestSequence = { 1, 2, 3 };

    static TestBase()
    {
        Serializer.MemberSerializing += OnMemberSerializing;
        Serializer.MemberSerialized += OnMemberSerialized;
        Serializer.MemberDeserializing += OnMemberDeserializing;
        Serializer.MemberDeserialized += OnMemberDeserialized;
    }

    public T Roundtrip<T>(T o, object context = null)
    {
        PrintSerialize(typeof(T));

        var stream = new MemoryStream();
        Serialize(stream, o, context);

        stream.Position = 0;

        PrintDeserialize(typeof(T));
        return Deserialize<T>(stream, context);
    }

    protected T Roundtrip<T>(T o, long expectedLength)
    {
        PrintSerialize(typeof(T));
        var stream = new MemoryStream();
        Serialize(stream, o);

        stream.Position = 0;
        var data = stream.ToArray();

        Assert.AreEqual(expectedLength, data.Length);

        PrintDeserialize(typeof(T));
        return Deserialize<T>(stream);
    }


    protected T RoundtripBigEndian<T>(T o, long expectedLength)
    {
        PrintSerialize(typeof(T));
        var stream = new MemoryStream();
        SerializeBe(stream, o);

        stream.Position = 0;
        var data = stream.ToArray();

        Assert.AreEqual(expectedLength, data.Length);

        PrintDeserialize(typeof(T));
        return DeserializeBe<T>(stream);
    }

    protected T Roundtrip<T>(T o, byte[] expectedValue)
    {
        PrintSerialize(typeof(T));
        var stream = new MemoryStream();
        Serialize(stream, o);

        stream.Position = 0;
        var data = stream.ToArray();

        AssertEqual(expectedValue, data);

        PrintDeserialize(typeof(T));
        return Deserialize<T>(stream);
    }

    protected T RoundtripBigEndian<T>(T o, byte[] expectedValue)
    {
        PrintSerialize(typeof(T));
        var stream = new MemoryStream();
        SerializeBe(stream, o);

        stream.Position = 0;
        var data = stream.ToArray();

        AssertEqual(expectedValue, data);

        PrintDeserialize(typeof(T));
        return DeserializeBe<T>(stream);
    }

    private void AssertEqual(byte[] expected, byte[] actual)
    {
        var length = Math.Min(expected.Length, actual.Length);

        for (var i = 0; i < length; i++)
        {
            var e = expected[i];
            var a = actual[i];

            Assert.IsTrue(e == a, $"Value at position {i} does not match expected value.  Expected 0x{e:X2}, got 0x{a:X2}");
        }

        Assert.IsTrue(expected.Length == actual.Length, $"Sequence lengths do not match.  Expected {expected.Length}, got {actual.Length}");
    }

    protected T RoundtripReverse<T>(byte[] data)
    {
        var o = Deserialize<T>(data);

        PrintSerialize(typeof(T));
        var stream = new MemoryStream();
        Serialize(stream, o);

        AssertEqual(data, stream.ToArray());

        return o;
    }

    protected byte[] Serialize(object o)
    {
        var stream = new MemoryStream();
        Serialize(stream, o);
        return stream.ToArray();
    }

    protected T Deserialize<T>(string filename)
    {
        using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
        {
            PrintDeserialize(typeof(T));
            return Deserialize<T>(stream);
        }
    }

    protected T Deserialize<T>(byte[] data)
    {
        PrintDeserialize(typeof(T));
        return Deserialize<T>(new MemoryStream(data));
    }

    protected T Deserialize<T>(Stream stream, object context = null)
    {
#if TESTASYNC
        var task = Serializer.DeserializeAsync<T>(stream, context);
        task.ConfigureAwait(false);
        task.Wait();
        return task.Result;
#else
            return Serializer.Deserialize<T>(stream, context);
#endif
    }

    protected void Serialize(Stream stream, object o, object context = null)
    {
#if TESTASYNC
        var task = Serializer.SerializeAsync(stream, o, context);
        task.ConfigureAwait(false);
        task.Wait();
#else
            Serializer.Serialize(stream, o, context);
#endif
    }

    protected void SerializeBe(Stream stream, object o)
    {
#if TESTASYNC
        var task = SerializerBe.SerializeAsync(stream, o);
        task.ConfigureAwait(false);
        task.Wait();
#else
            SerializerBe.Serialize(stream, o);
#endif
    }

    protected T DeserializeBe<T>(Stream stream)
    {
#if TESTASYNC
        var task = SerializerBe.DeserializeAsync<T>(stream);
        task.ConfigureAwait(false);
        task.Wait();
        return task.Result;
#else
            return SerializerBe.Deserialize<T>(stream);
#endif
    }

    private static void PrintIndent(int depth)
    {
        var indent = new string(Enumerable.Repeat(' ', depth * 4).ToArray());
        Debug.Write(indent);
    }

    private static void PrintSerialize(Type type)
    {
        Debug.WriteLine($"S-{type}");
    }

    private static void PrintDeserialize(Type type)
    {
        Debug.WriteLine($"D-{type}");
    }

    private static void OnMemberSerializing(object sender, MemberSerializingEventArgs e)
    {
        PrintIndent(e.Context.Depth);
        Debug.WriteLine("S-Start: {0} @ {1}", e.MemberName, e.Offset);
    }

    private static void OnMemberSerialized(object sender, MemberSerializedEventArgs e)
    {
        PrintIndent(e.Context.Depth);
        var value = e.Value ?? "null";
        Debug.WriteLine("S-End: {0} ({1}) @ {2}", e.MemberName, value, e.Offset);
    }

    private static void OnMemberDeserializing(object sender, MemberSerializingEventArgs e)
    {
        PrintIndent(e.Context.Depth);
        Debug.WriteLine("D-Start: {0} @ {1}", e.MemberName, e.Offset);
    }

    private static void OnMemberDeserialized(object sender, MemberSerializedEventArgs e)
    {
        PrintIndent(e.Context.Depth);
        var value = e.Value ?? "null";

        if (value is byte[])
        {
            value = BitConverter.ToString((byte[])value);
        }

        Debug.WriteLine("D-End: {0} ({1}) @ {2}", e.MemberName, value, e.Offset);
    }
}
