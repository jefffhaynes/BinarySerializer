using System;
using System.IO;

namespace BinarySerialization
{
    /// <summary>
    /// Represents an exception that will be thrown due to write access
    /// </summary>
    public class ReadOnlyStreamException: IOException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyStreamException"/> class
        /// </summary>
        public ReadOnlyStreamException() 
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyStreamException"/> class
        /// </summary>
        /// <param name="message">A String that describes the error. </param>
        public ReadOnlyStreamException(string message) : base(message) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyStreamException"/> class
        /// </summary>
        /// <param name="message">A String that describes the error.</param>
        /// <param name="innerException">The inner exception reference.</param>
        public ReadOnlyStreamException(string message, Exception innerException) : base(message, innerException) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyStreamException"/> class
        /// </summary>
        /// <param name="message">A String that describes the error.</param>
        /// <param name="hresult">An integer identifying the error that has occurred</param>
        public ReadOnlyStreamException(string message, int hresult) : base(message, hresult) 
        {
        }
    }
}