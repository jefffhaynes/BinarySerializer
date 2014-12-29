using System;

namespace BinarySerialization
{
    /// <summary>
    /// Specifies an explicit list termination condition when the condition is defined external to the items in
    /// the collection (e.g. a null-terminated list).
    /// </summary>
    /// <example>
    /// Here's a real-world example from the ISO 9660 specification in which directory records simply repeat until they don't anymore.
    /// Technically, the specification reads that a directory entry with a length of zero terminates the collection.  However, since
    /// it isn't possible to construct a well-formed directory entry with length of zero, this isn't a useful definition for our purposes.
    /// Therefore, we have to consider the termination of the collection external to the items themselves, resulting in what is essentially
    /// a null- or zero-terminated collection.
    /// <code>
    /// public class PathTableRecord
    /// {
    ///    public byte NameLength { get; set; }
    ///    public byte ExtendedAttributeRecordSectors { get; set; }
    ///    public uint FirstSector { get; set; }
    ///    public ushort ParentDirectoryRecord { get; set; }
    ///
    ///    [FieldLength("NameLength")]
    ///    public string Name { get; set; }
    ///
    ///    [FieldLength("PaddingLength")]
    ///    public byte[] Padding { get; set; }
    ///
    ///    [Ignore]
    ///    public int PaddingLength
    ///    {
    ///        get { return NameLength % 2; }
    ///    }
    ///
    ///    [FieldOffset("FirstSector", ConverterType = typeof(SectorByteConverter))]
    ///    [SerializeUntil((byte)0)]
    ///    public List&lt;DirectoryRecord&gt; Directories { get; set; }
    /// }
    /// 
    /// public class DirectoryRecord
    /// {
    ///    public byte Length { get; set; }
    ///    public byte ExtendedAttributeRecordSectors { get; set; }
    /// 
    ///    // ...
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false)]
    public sealed class SerializeUntilAttribute : FieldBindingBaseAttribute, IConstAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializeUntilAttribute"/> class with a terminating constValue.
        /// </summary>
        /// <param name="constValue"></param>
        public SerializeUntilAttribute(object constValue)
        {
            ConstValue = constValue;
        }
		
        /// <summary>
        /// The terminating constValue.
        /// </summary>
        public object ConstValue { get; set; }

        internal override bool IsConstSupported
        {
            get { return true; }
        }

        public object GetConstValue()
        {
            return ConstValue;
        }
    }
}
