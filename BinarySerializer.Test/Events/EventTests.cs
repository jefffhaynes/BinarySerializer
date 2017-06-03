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
            Assert.Equal(0, events[0].Offset);

            Assert.Equal("Length", events[1].MemberName);
            Assert.Equal(typeof(MemberSerializedEventArgs), events[1].GetType());
            Assert.Equal(4, events[1].Offset);

            Assert.Equal("InnerClass", events[2].MemberName);
            Assert.Equal(typeof(MemberSerializingEventArgs), events[2].GetType());
            Assert.Equal(4, events[2].Offset);

            Assert.Equal("Length", events[3].MemberName);
            Assert.Equal(typeof(MemberSerializingEventArgs), events[3].GetType());
            Assert.Equal(4, events[3].Offset);

            Assert.Equal("Length", events[4].MemberName);
            Assert.Equal(typeof(MemberSerializedEventArgs), events[4].GetType());
            Assert.Equal(8, events[4].Offset);

            Assert.Equal("InnerClass", events[5].MemberName);
            Assert.Equal(typeof(MemberSerializingEventArgs), events[5].GetType());
            Assert.Equal(8, events[5].Offset);

            Assert.Equal("Value", events[6].MemberName);
            Assert.Equal(typeof(MemberSerializingEventArgs), events[6].GetType());
            Assert.Equal(8, events[6].Offset);

            Assert.Equal("Value", events[7].MemberName);
            Assert.Equal(typeof(MemberSerializedEventArgs), events[7].GetType());
            Assert.Equal(10, events[7].Offset);

            Assert.Equal("InnerClass", events[8].MemberName);
            Assert.Equal(typeof (MemberSerializedEventArgs), events[8].GetType());
            Assert.Equal(10, events[8].Offset);

            Assert.Equal("InnerClass", events[9].MemberName);
            Assert.Equal(typeof (MemberSerializedEventArgs), events[9].GetType());
            Assert.Equal(10, events[9].Offset);
        }
    }
}