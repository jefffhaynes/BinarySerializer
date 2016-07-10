namespace BinarySerialization.Test
{
    public class NullArrayClass
    {
        public int LastMember;

        [FieldLength(24)]
        public byte[] Filler { get; set; }
    }
}