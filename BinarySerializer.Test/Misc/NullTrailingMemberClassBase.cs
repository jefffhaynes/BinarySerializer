using BinarySerialization;

namespace BinarySerializer.Test.Misc
{
    public class NullTrailingMemberClassBase
    {
        [SerializeAs(Order = -1)]
        public int BaseValue { get; set; }
    }
}
