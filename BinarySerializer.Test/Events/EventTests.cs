namespace BinarySerialization.Test.Events;

[TestClass]
public class EventTests
{
    [TestMethod]
    public void TestSerializeEvents()
    {
        var serializer = new BinarySerializer();

        var events = new List<MemberSerializingEventArgs>();

        serializer.MemberSerializing += (sender, args) => events.Add(args);
        serializer.MemberSerialized += (sender, args) => events.Add(args);

        var stream = new MemoryStream();
        serializer.Serialize(stream,
            new EventTestClass
            {
                InnerClass = new EventTestInnerClass
                {
                    InnerClass = new EventTestInnerInnerClass()
                }
            });

        Assert.AreEqual("Length", events[0].MemberName);
        Assert.AreEqual(typeof(MemberSerializingEventArgs), events[0].GetType());
        Assert.AreEqual(0, (int)events[0].Offset.ByteCount);
        Assert.AreEqual(0, (int)events[0].LocalOffset.ByteCount);

        Assert.AreEqual("Length", events[1].MemberName);
        Assert.AreEqual(typeof(MemberSerializedEventArgs), events[1].GetType());
        Assert.AreEqual(4, (int)events[1].Offset.ByteCount);
        Assert.AreEqual(4, (int)events[1].LocalOffset.ByteCount);

        Assert.AreEqual("InnerClass", events[2].MemberName);
        Assert.AreEqual(typeof(MemberSerializingEventArgs), events[2].GetType());
        Assert.AreEqual(4, (int)events[2].Offset.ByteCount);
        Assert.AreEqual(4, (int)events[2].LocalOffset.ByteCount);

        Assert.AreEqual("Length", events[3].MemberName);
        Assert.AreEqual(typeof(MemberSerializingEventArgs), events[3].GetType());
        Assert.AreEqual(4, (int)events[3].Offset.ByteCount);
        Assert.AreEqual(0, (int)events[3].LocalOffset.ByteCount);

        Assert.AreEqual("Length", events[4].MemberName);
        Assert.AreEqual(typeof(MemberSerializedEventArgs), events[4].GetType());
        Assert.AreEqual(8, (int)events[4].Offset.ByteCount);
        Assert.AreEqual(4, (int)events[4].LocalOffset.ByteCount);

        Assert.AreEqual("InnerClass", events[5].MemberName);
        Assert.AreEqual(typeof(MemberSerializingEventArgs), events[5].GetType());
        Assert.AreEqual(8, (int)events[5].Offset.ByteCount);
        Assert.AreEqual(4, (int)events[5].LocalOffset.ByteCount);

        Assert.AreEqual("Value", events[6].MemberName);
        Assert.AreEqual(typeof(MemberSerializingEventArgs), events[6].GetType());
        Assert.AreEqual(8, (int)events[6].Offset.ByteCount);
        Assert.AreEqual(0, (int)events[6].LocalOffset.ByteCount);

        Assert.AreEqual("Value", events[7].MemberName);
        Assert.AreEqual(typeof(MemberSerializedEventArgs), events[7].GetType());
        Assert.AreEqual(10, (int)events[7].Offset.ByteCount);
        Assert.AreEqual(2, (int)events[7].LocalOffset.ByteCount);

        Assert.AreEqual("InnerClass", events[8].MemberName);
        Assert.AreEqual(typeof(MemberSerializedEventArgs), events[8].GetType());
        Assert.AreEqual(10, (int)events[8].Offset.ByteCount);
        Assert.AreEqual(6, (int)events[8].LocalOffset.ByteCount);

        Assert.AreEqual("InnerClass", events[9].MemberName);
        Assert.AreEqual(typeof(MemberSerializedEventArgs), events[9].GetType());
        Assert.AreEqual(10, (int)events[9].Offset.ByteCount);
        Assert.AreEqual(10, (int)events[9].LocalOffset.ByteCount);
    }
}
