using System.Collections.Generic;
using System.IO;
using Xunit;

namespace BinarySerialization.Test.Events
{
    
    public class EventTests
    {
        [Fact]
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
            
            Assert.Equal("Length", events[0].MemberName);
            Assert.Equal(typeof (MemberSerializingEventArgs), events[0].GetType());
            Assert.Equal(0, (int) events[0].Offset.ByteCount);
            Assert.Equal(0, (int) events[0].LocalOffset.ByteCount);

            Assert.Equal("Length", events[1].MemberName);
            Assert.Equal(typeof(MemberSerializedEventArgs), events[1].GetType());
            Assert.Equal(4, (int) events[1].Offset.ByteCount);
            Assert.Equal(4, (int) events[1].LocalOffset.ByteCount);

            Assert.Equal("InnerClass", events[2].MemberName);
            Assert.Equal(typeof(MemberSerializingEventArgs), events[2].GetType());
            Assert.Equal(4, (int) events[2].Offset.ByteCount);
            Assert.Equal(4, (int) events[2].LocalOffset.ByteCount);

            Assert.Equal("Length", events[3].MemberName);
            Assert.Equal(typeof(MemberSerializingEventArgs), events[3].GetType());
            Assert.Equal(4, (int) events[3].Offset.ByteCount);
            Assert.Equal(0, (int) events[3].LocalOffset.ByteCount);

            Assert.Equal("Length", events[4].MemberName);
            Assert.Equal(typeof(MemberSerializedEventArgs), events[4].GetType());
            Assert.Equal(8, (int) events[4].Offset.ByteCount);
            Assert.Equal(4, (int) events[4].LocalOffset.ByteCount);

            Assert.Equal("InnerClass", events[5].MemberName);
            Assert.Equal(typeof(MemberSerializingEventArgs), events[5].GetType());
            Assert.Equal(8, (int) events[5].Offset.ByteCount);
            Assert.Equal(4, (int) events[5].LocalOffset.ByteCount);

            Assert.Equal("Value", events[6].MemberName);
            Assert.Equal(typeof(MemberSerializingEventArgs), events[6].GetType());
            Assert.Equal(8, (int) events[6].Offset.ByteCount);
            Assert.Equal(0, (int) events[6].LocalOffset.ByteCount);

            Assert.Equal("Value", events[7].MemberName);
            Assert.Equal(typeof(MemberSerializedEventArgs), events[7].GetType());
            Assert.Equal(10, (int) events[7].Offset.ByteCount);
            Assert.Equal(2, (int) events[7].LocalOffset.ByteCount);

            Assert.Equal("InnerClass", events[8].MemberName);
            Assert.Equal(typeof (MemberSerializedEventArgs), events[8].GetType());
            Assert.Equal(10, (int) events[8].Offset.ByteCount);
            Assert.Equal(6, (int) events[8].LocalOffset.ByteCount);

            Assert.Equal("InnerClass", events[9].MemberName);
            Assert.Equal(typeof (MemberSerializedEventArgs), events[9].GetType());
            Assert.Equal(10, (int) events[9].Offset.ByteCount);
            Assert.Equal(10, (int) events[9].LocalOffset.ByteCount);
        }
    }
}