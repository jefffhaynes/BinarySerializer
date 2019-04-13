using System;
using System.Text;

namespace BinarySerialization
{
    internal static class EncodingHelper
    {
        public static Encoding GetEncoding(string name)
        {
            if (name.Equals("UTF-16", StringComparison.OrdinalIgnoreCase))
            {
                return new UnicodeEncoding(false, false);
            }

            if (name.Equals("UTF-8", StringComparison.OrdinalIgnoreCase))
            {
                return new UTF8Encoding(false);
            }

            return Encoding.GetEncoding(name);
        }
    }
}
