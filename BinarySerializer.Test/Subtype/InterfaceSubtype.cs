namespace BinarySerialization.Test.Subtype
{
    public interface ISubtype
    {
        public string Value { get; set; }
    }

    public class InterfaceSubclassA : ISubtype
    {
        [FieldOrder(0)]
        public string Value { get; set; }

        [FieldOrder(1)]
        public int MoreStuff { get; set; }
    }

    public class InterfaceSubclassB : ISubtype
    {
        public string Value { get; set; }
    }

    public class InterfaceSubtype
    {
        [FieldOrder(0)]
        public byte Indicator { get; set; }

        [FieldOrder(1)]
        public byte Length { get; set; }

        [FieldOrder(2)]
        [FieldLength(nameof(Length))]
        [Subtype(nameof(Indicator), 1, typeof(InterfaceSubclassA))]
        [Subtype(nameof(Indicator), 2, typeof(InterfaceSubclassB))]
        public ISubtype Value { get; set; }
    }
}
