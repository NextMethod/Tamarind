using System;
using System.Linq;

using Tamarind.Annotations;

namespace Tamarind.Concurrent
{
    public interface IStriped<out T>
    {

        /// <summary>
        ///     Returns the stripe at the specified index. Valid indexes are 0, inclusively, to <see cref="Size" />, exclusively.
        /// </summary>
        /// <param name="index">The index of the stripe to return; must be in <c>[0...Size]</c></param>
        /// <returns>The stripe at the specified index</returns>
        [PublicAPI]
        T this[int index] { get; }

        /// <summary>
        ///     The total number of stripes in this instance.
        /// </summary>
        [PublicAPI]
        int Size { get; }

        /// <summary>
        ///     Returns the stripe that corresponds to the passed key. It is always guaranteed that if
        ///     <c>key1.Equals(key2)</c>, then <c>Get(key1) == Get(key2)</c>.
        /// </summary>
        /// <param name="key">An arbitrary, non-null key</param>
        /// <returns>The stripe that the passed key corresponds to</returns>
        [PublicAPI]
        T Get(object key);

    }
}
