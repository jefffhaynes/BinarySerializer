using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Binding
{
    [TestClass]
    public class PreviousBindingTests : TestBase
    {
        [TestMethod]
        public void PreviousBindingTest()
        {
            var expected = new PreviousBindingClass
            {
                Items = new List<PreviousBindingClassItem>
                {
                    new PreviousBindingClassItem(),
                    new PreviousBindingClassItem
                    {
                        Value = "test"
                    }
                }
            };

            var actual = Roundtrip(expected);
        }
    }
}
