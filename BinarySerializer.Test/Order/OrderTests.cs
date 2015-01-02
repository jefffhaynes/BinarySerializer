using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerializer.Test.Order
{
    [TestClass]
    public class OrderTests : TestBase
    {
        [TestMethod]
        public void OrderTest()
        {
            var order = new OrderClass {First = 1, Second = 2};
            Roundtrip(order, new byte[] {0x1, 0x2});
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
            var order = new MutlipleMembersInvalidOrderClass();
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
