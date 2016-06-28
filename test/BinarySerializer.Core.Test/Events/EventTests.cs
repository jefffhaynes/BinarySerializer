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
            serializer.Serialize(stream, new EventTestClass {InnerClass = new EventTestInnerClass()});

            // check types
            Assert.Equal(typeof(MemberSerializingEventArgs), events[0].GetType());
            Assert.Equal(typeof(MemberSerializingEventArgs), events[1].GetType());
            Assert.Equal(typeof(MemberSerializedEventArgs), events[2].GetType());
            Assert.Equal(typeof(MemberSerializedEventArgs), events[3].GetType());

            // check names
            Assert.Equal("InnerClass", events[0].MemberName);
            Assert.Equal("Value", events[1].MemberName);
            Assert.Equal("Value", events[2].MemberName);
            Assert.Equal("InnerClass", events[3].MemberName);

            // check offsets
            Assert.Equal(0, events[0].Offset);
            Assert.Equal(0, events[1].Offset);
            Assert.Equal(4, events[2].Offset);
            Assert.Equal(4, events[3].Offset);
        }
    }
}
