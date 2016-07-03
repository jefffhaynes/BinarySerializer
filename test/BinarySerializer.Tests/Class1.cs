using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using BinarySerialization;

namespace BinarySerialization.Tests
{
    public class SampleTest
    {
        [Fact]
        public void PassingTest()
        {
						BinarySerializer bs = new BinarySerializer();
            Assert.Equal(4, Add(2, 2));
        }

        [Fact]
        public void FailingTest()
        {
            Assert.Equal(5, Add(2, 2));
        }

        int Add(int x, int y)
        {
            return x + y;
        }
    }
}
