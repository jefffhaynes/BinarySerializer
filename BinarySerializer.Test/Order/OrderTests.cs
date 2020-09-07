using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Order
{
    [TestClass]
    public class OrderTests : TestBase
    {
        [TestMethod]
        public void OrderTest()
        {
            var order = new OrderClass {First = 1, Second = 2, Name = "Alice"};
            Roundtrip(order, new byte[] {0x1, 0x2, 0x5, 0x41, 0x6c, 0x69, 0x63, 0x65});
        }

        [TestMethod]
        public void SingleMemberOrderShouldntThrowTest()
        {
            var order = new SingleMemberOrderClass();
            Roundtrip(order);
        }

        [TestMethod]
        public void MultipleMembersNoOrderAttributeShouldThrowTest()
        {
            var order = new MutlipleMembersNoOrderClass();

#if TESTASYNC
            Assert.ThrowsException<AggregateException>(() => Roundtrip(order));
#else
            Assert.ThrowsException<InvalidOperationException>(() => Roundtrip(order));
#endif
        }

        [TestMethod]
        public void MultipleMembersDuplicateOrderAttributeShouldThrowTest()
        {
            var order = new MutlipleMembersDuplicateOrderClass();

#if TESTASYNC
            Assert.ThrowsException<AggregateException>(() => Roundtrip(order));
#else
            Assert.ThrowsException<InvalidOperationException>(() => Roundtrip(order));
#endif
        }

        [TestMethod]
        public void BaseClassComesBeforeDerivedClassTest()
        {
            var order = new OrderDerivedClass {First = 1, Second = 2};
            Roundtrip(order, new byte[] {0x1, 0x2});
        }
    }
}