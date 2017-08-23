using System.Collections.Generic;

namespace BinarySerialization.Test.ReadOnlyListStreamTests
{
    public abstract class TestBase
    {
        protected readonly IReadOnlyList<byte> _emptyList;
        protected readonly IReadOnlyList<byte> _list;

        protected TestBase()
        {
            _emptyList = new List<byte>();

            _list = new List<byte> {
                0x00, 0x01, 0x02, 0x03, 0x04,
                0x05, 0x06, 0x07, 0x08, 0x09
            };
        }
    }
}