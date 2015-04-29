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
        [ExpectedException(typeof(InvalidOperationException))]
        public void MultipleMembersNoOrderAttributeShouldThrowTest()
        {
            var order = new MutlipleMembersNoOrderClass();
            Roundtrip(order);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MultipleMembersDuplicateOrderAttributeShouldThrowTest()
        {
            var order = new MutlipleMembersDuplicateOrderClass();
            Roundtrip(order);
        }

        [TestMethod]
        public void BaseClassComesBeforeDerivedClassTest()
        {
            var order = new OrderDerivedClass { First = 1, Second = 2 };
            Roundtrip(order, new byte[] { 0x1, 0x2 });
        }
    }
}
