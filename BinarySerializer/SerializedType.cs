using System;

namespace BinarySerialization
{
    /// <summary>
    /// Used to control how primitive values are serialized and interpreted.
    /// <seealso cref="BinarySerializer"/>
    /// </summary>
    public enum SerializedType
    {
        /// <summary>
        /// Used to specify that the default encoding for a given type be used.
        /// </summary>
        Default,

        /// <summary>
        /// An 8-bit signed integer ranging in value from -128 to 127.
        /// </summary>
        Int1,

        /// <summary>
        /// An 8-bit unsigned integer ranging in value from 0 to 255.
        /// </summary>
        UInt1,

        /// <summary>
        /// An integral type representing signed 16-bit integers with values between -32768 and 32767.
        /// </summary>
        Int2,

        /// <summary>
        /// An integral type representing unsigned 16-bit integers with values between 0 and 65535.
        /// </summary>
        UInt2,

        /// <summary>
        /// An integral type representing signed 32-bit integers with values between -2147483648 and 2147483647.
        /// </summary>
        Int4,
        
        /// <summary>
        /// An integral type representing unsigned 32-bit integers with values between 0 and 4294967295.
        /// </summary>
        UInt4,

        /// <summary>
        /// An integral type representing signed 64-bit integers with values between -9223372036854775808 and 9223372036854775807.
        /// </summary>
        Int8,

        /// <summary>
        /// An integral type representing unsigned 64-bit integers with values between 0 and 18446744073709551615.
        /// </summary>
        UInt8,

        /// <summary>
        /// A floating point type representing values ranging from approximately 1.5 x 10 <sup>-45</sup> to 
        /// 3.4 x 10 <sup>38</sup> with a precision of 7 digits expressed in IEEE format.
        /// </summary>
        Float4,

        /// <summary>
        /// A floating point type representing values ranging from approximately 5.0 x 10 <sup>-324</sup> to 
        /// 1.7 x 10 <sup>308</sup> with a precision of 15-16 digits in IEEE format.
        /// </summary>
        Float8,

        /// <summary>
        /// An array of bytes with a length specified by <see cref="FieldLengthAttribute"/>.
        /// </summary>
        ByteArray,

        /// <summary>
        /// A fixed-size string with a length specified by <see cref="FieldLengthAttribute"/>.
        /// </summary>
        SizedString,

        /// <summary>
        /// An encoded string with a null (zero byte) termination.
        /// </summary>
        [Obsolete("Use TerminatedString")]
        NullTerminatedString,

        /// <summary>
        /// An encoded string with a null (zero byte) termination.
        /// </summary>
        TerminatedString,

        /// <summary>
        /// An encoded string prefixed with a LEB128-encoded length.  This is equivalent to how BinaryWriter encodes a string.
        /// </summary>
        LengthPrefixedString,
    }
}
