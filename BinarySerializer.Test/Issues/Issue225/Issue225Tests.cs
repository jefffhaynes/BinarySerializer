using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySerialization.Test.Issues.Issue225
{
    [TestClass]
    public class Issue225Tests : TestBase
    {
        [TestMethod]
        public void Test()
        {
            var value = new ValueDataInfo
            {
                BlockType = ValueBlockType.PlainValue,
                ParameterId = 1,
                DataTypeId = ValueDataType.Datatype_Int32,
                Block = new Int32PlainValuesDataBody { Data = new List<int> { 1, 2, 3 } }
            };

            Roundtrip(value);
        }
    }
}
