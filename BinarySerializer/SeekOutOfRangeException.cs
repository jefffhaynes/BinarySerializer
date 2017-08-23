using System;
using System.IO;

namespace BinarySerialization {
    /// <summary>
    /// Represents an exception that will be thrown due to exceeded boundary access
    /// </summary>
    public class SeekOutOfRangeException : IOException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeekOutOfRangeException"/> class
        /// </summary>
        public SeekOutOfRangeException() 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeekOutOfRangeException"/> class
        /// </summary>
        /// <param name="message">A String that describes the error.</param>
        public SeekOutOfRangeException(string message) : base(message) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeekOutOfRangeException"/> class
        /// </summary>
        /// <param name="message">A String that describes the error.</param>
        /// <param name="innerException">The inner exception reference.</param>
        public SeekOutOfRangeException(string message, Exception innerException) : base(message, innerException) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeekOutOfRangeException"/> class
        /// </summary>
        /// <param name="message">A String that describes the error.</param>
        /// <param name="hresult">An integer identifying the error that has occurred</param>
        public SeekOutOfRangeException(string message, int hresult) : base(message, hresult)
        {
        }
    }
}