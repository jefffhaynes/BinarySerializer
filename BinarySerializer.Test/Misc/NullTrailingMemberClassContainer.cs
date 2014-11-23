using BinarySerialization;

namespace BinarySerializer.Test.Misc
{
    public class NullTrailingMemberClassContainer
    {
        public NullTrailingMemberClassContainer()
        {
            Inner = new NullTrailingMemberClass();
        }

        public int InnerLength { get; set; }

        [FieldLength("InnerLength")]
        public NullTrailingMemberClass Inner { get; set; }
    }
}
