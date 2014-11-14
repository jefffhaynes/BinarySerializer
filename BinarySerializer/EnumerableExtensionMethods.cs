using System;
using System.Collections.Generic;

namespace BinarySerialization
{
    internal static class EnumerableExtensionMethods
    {
        public static bool? Consensus<TSource>(this IEnumerable<TSource> sequence, Func<TSource, bool> predicate)
        {
            bool? first = null;
            bool firstIteration = true;
            foreach (var item in sequence)
            {
                if (firstIteration)
                {
                    first = predicate(item);
                    firstIteration = false;
                }
                else
                {
                    var result = predicate(item);
                    if (!first.Equals(result))
                        return null;
                }
            }

            return first;
        }
    }
}
