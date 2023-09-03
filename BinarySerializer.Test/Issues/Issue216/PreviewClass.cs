namespace BinarySerialization.Test.Issues.Issue216
{
    public class Preview
    {
        /// <summary>
        /// Gets the image width, in pixels.
        /// </summary>
        [FieldOrder(0)]
        public uint ResolutionX { get; set; }

        /// <summary>
        /// Gets the image height, in pixels.
        /// </summary>
        [FieldOrder(2)]
        public uint ResolutionY { get; set; }

        [Ignore]
        public uint DataSize => ResolutionX * ResolutionY * 2;

        [FieldOrder(3)]
        [FieldCount(nameof(DataSize), BindingMode = BindingMode.OneWay)]
        public byte[] Data { get; set; }

        [FieldOrder(4)]
        public uint Empty;
    }
}
