using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using BinarySerialization;

namespace bs.tests
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dnx.html
    public class SampleTest
    {
        [Fact]
        public void PassingTest()
        {
            Assert.Equal(4, Add(2, 2));
        }

        [Fact]
        public void FailingTest()
        {
						BinarySerializer bs = new BinarySerializer();
            Assert.Equal(5, Add(2, 2));
        }

        int Add(int x, int y)
        {
            return x + y;
        }
    }
}
