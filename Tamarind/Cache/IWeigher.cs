using System;
using System.Linq;

namespace Tamarind.Cache
{
    /// <summary>
    ///     Calculates the weights of cache entries.
    /// </summary>
    public interface IWeigher<in TKey, in TValue>
    {

        /// <summary>
        ///     Returns the weight of a cache entry. There is no unit for entry weights; rather they are simply relative to each
        ///     other.
        /// </summary>
        /// <returns>the weight of the entry; must be non-negative</returns>
        int Weigh(TKey key, TValue value);

    }
}
