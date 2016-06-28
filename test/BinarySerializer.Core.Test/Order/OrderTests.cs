using System;
using Xunit;

namespace BinarySerialization.Test.Order
{
        public class OrderTests : TestBase
    {
        [Fact]
        public void OrderTest()
        {
            var order = new OrderClass {First = 1, Second = 2, Name = "Alice"};
            Roundtrip(order, new byte[] {0x1, 0x2, 0x5, 0x41, 0x6c, 0x69, 0x63, 0x65});
        }

        [Fact]
        public void SingleMemberOrderShouldntThrowTest()
        {
            var order = new SingleMemberOrderClass();
            Roundtrip(order);
        }

        [Fact]
        //[ExpectedException(typeof(InvalidOperationException))]
        public void MultipleMembersNoOrderAttributeShouldThrowTest()
        {
            var order = new MutlipleMembersNoOrderClass();
            var e = Record.Exception(() => Roundtrip(order));
            Assert.NotNull(e);
            Assert.IsType<InvalidOperationException>(e);
        }

        [Fact]
        //[ExpectedException(typeof(InvalidOperationException))]
        public void MultipleMembersDuplicateOrderAttributeShouldThrowTest()
        {
            var order = new MutlipleMembersDuplicateOrderClass();
            var e = Record.Exception(() => Roundtrip(order));
            Assert.NotNull(e);
            Assert.IsType<InvalidOperationException>(e); 
        }

        [Fact]
        public void BaseClassComesBeforeDerivedClassTest()
        {
            var order = new OrderDerivedClass { First = 1, Second = 2 };
            Roundtrip(order, new byte[] { 0x1, 0x2 });
        }
    }
}
