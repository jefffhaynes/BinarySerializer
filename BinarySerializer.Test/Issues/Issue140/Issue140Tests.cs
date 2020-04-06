using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BinarySerialization.Test.Issues.Issue140
{
    public class Issue140Tests : TestBase
    {
        [Fact]
        public void Test()
        {
            var expected = new Question
            {
                Domain = new Domain()
            };

            var actual = Roundtrip(expected);
        }
    }
}
