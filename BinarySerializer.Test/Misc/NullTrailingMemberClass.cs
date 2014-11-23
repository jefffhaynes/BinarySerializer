namespace BinarySerializer.Test.Misc
{
    public class NullTrailingMemberClass : NullTrailingMemberClassBase
    {
        public int Value { get; set; }

        public byte? OptionalParameter { get; set; }
    }
}
