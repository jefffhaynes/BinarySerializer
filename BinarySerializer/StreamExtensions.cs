using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BinarySerialization
{
    public static class StreamExtensions
    {
        public static void CopyTo(this Stream input, Stream output, int length, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            int read;
            while (length > 0 &&
                   (read = input.Read(buffer, 0, Math.Min(buffer.Length, length))) > 0)
            {
                output.Write(buffer, 0, read);
                length -= read;
            }
        }

        public static async Task CopyToAsync(this Stream input, Stream output, int length, int bufferSize, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[bufferSize];
            int read;
            while (length > 0 &&
                   (read = await input.ReadAsync(buffer, 0, Math.Min(buffer.Length, length), cancellationToken)
                       .ConfigureAwait(false)) > 0)
            {
                await output.WriteAsync(buffer, 0, read, cancellationToken).ConfigureAwait(false);
                length -= read;
            }
        }
    }
}
