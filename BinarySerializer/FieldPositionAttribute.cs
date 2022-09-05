using System;
using System.IO;

namespace BinarySerialization
{
    /// <summary>
    ///     Specifies an absolute offset of a member in the stream.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FieldPositionAttribute : FieldBindingBaseAttribute, IConstAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the FieldPosition attribute with a fixed seek.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="seekOrigin"></param>
        /// <param name="rewind"></param>
        public FieldPositionAttribute(ulong offset, SeekOrigin seekOrigin = SeekOrigin.Begin, bool rewind = false)
        {
            ConstOffset = offset;
            SeekOrigin = seekOrigin;
            Rewind = rewind;
        }

        /// <summary>
        ///     Initializes a new instance of the FieldPosition attribute with a path pointing to a source binding member.
        /// </summary>
        /// <param name="path">A path to the source member.</param>
        /// <param name="seekOrigin"></param>
        /// <param name="rewind"></param>
        public FieldPositionAttribute(string path, SeekOrigin seekOrigin = SeekOrigin.Begin, bool rewind = false) : base(path)
        {
            SeekOrigin = seekOrigin;
            Rewind = rewind;
        }

        /// <summary>
        ///     Initializes a new instance of the FieldPosition attribute with a fixed seek position.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="rewind"></param>
        /// <param name="seekOrigin"></param>
        public FieldPositionAttribute(ulong offset, bool rewind, SeekOrigin seekOrigin = SeekOrigin.Begin) : this(offset, seekOrigin, rewind) { }


        /// <summary>
        ///     Initializes a new instance of the FieldPosition attribute with a path pointing to a source binding member.
        /// </summary>
        /// <param name="path">A path to the source member.</param>
        /// <param name="rewind"></param>
        /// <param name="seekOrigin"></param>
        public FieldPositionAttribute(string path, bool rewind, SeekOrigin seekOrigin = SeekOrigin.Begin) : this(path, seekOrigin, rewind) { }

        /// <summary>
        ///     Used to specify fixed member offsets.
        /// </summary>
        public ulong ConstOffset { get; set; }

        /// <summary>
        ///     Get constant value or null if not constant.
        /// </summary>
        public object GetConstValue()
        {
            return ConstOffset;
        }

        /// <summary>
        ///     Specifies the position in a stream to use for seeking the field
        /// </summary>
        public SeekOrigin SeekOrigin { get; set; }

        /// <summary>
        ///     If true it will seek back to position where it was before seek, otherwise stream will continue from the current position
        /// </summary>
        public bool Rewind { get; set; }
    }
}