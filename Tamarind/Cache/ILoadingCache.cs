using System;
using System.Collections.Generic;
using System.Linq;

using Tamarind.Annotations;

namespace Tamarind.Cache
{

    /// <summary>
    ///     A semi-persistent mapping from keys to values. Values are automatically loaded by the cache, and are stored in the
    ///     cache until either evicted or manually invalidated.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Implementations of this interface are expected to be thread-safe, and can be safely accessed by multiple
    ///         concurrent threads.
    ///     </para>
    ///     <para>
    ///         See
    ///         <a href="http://docs.guava-libraries.googlecode.com/git-history/release/javadoc/com/google/common/cache/LoadingCache.html">
    ///             Guava
    ///             Reference
    ///         </a>
    ///     </para>
    /// </remarks>
    public interface ILoadingCache<TKey, TValue> : ICache<TKey, TValue>
    {

        /// <summary>
        ///     Returns the value associated with <paramref name="key" /> in this cache, first loading that value if necessary. No
        ///     observable state associated with this cache is modified until loading completes.
        ///     <para>
        ///         If another call to <see cref="Get" /> is currently loading the value for <paramref name="key" />, the cache
        ///         simply waits for that thread to finish and returns its loaded value. Note that multiple threads can
        ///         concurrently load values for distinct keys.
        ///     </para>
        ///     <para>
        ///         Caches loaded by an <see cref="ICacheLoader{TKey, TValue}" /> will call
        ///         <see cref="ICacheLoader{TKey,TValue}.Load" /> to new values into the cache. Newly loaded values are added to
        ///         the cache using <c>ICache{TKey,TValue}.ToDictionary().TryAdd()</c> after loading has
        ///         completed; if another value was associated with <paramref name="key" /> while the new value was loading then a
        ///         removal notification will be sent for the new value.
        ///     </para>
        /// </summary>
        [PublicAPI]
        TValue Get(TKey key);

        /// <summary>
        ///     Returns a dictionary of the values associated with <paramref name="keys" />, creating or retrieving those values if
        ///     necessary. The returned dictionary contains entries that were already cached, combined with newly loaded entries;
        ///     it will never contain null keys or values.
        ///     <para>
        ///         Caches loaded by a <see cref="ICacheLoader{TKey,TValue}" /> will issue a single request to
        ///         <see cref="ICacheLoader{TKey,TValue}.LoadAll" /> for all keys which are not already present in the cache. All
        ///         entries returned by <see cref="ICacheLoader{TKey,TValue}.LoadAll" /> will be stored in the cache, over-writing
        ///         any previously cached values. This method will throw an exception if
        ///         <see cref="ICacheLoader{TKey,TValue}.LoadAll" /> returns <c>null</c>, returns a dictionary containing
        ///         <c>null</c> keys or values, or fails to return an entry for each requested key.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Note that duplicate elements in <paramref name="keys" />, will be ignored.
        ///     </para>
        /// </remarks>
        [PublicAPI]
        IReadOnlyDictionary<TKey, TValue> GetAll(IEnumerable<TKey> keys);

        /// <summary>
        ///     Loads a new value for key <paramref name="key" />. While the new value is loading the previous value (if any) will
        ///     continue to be returned by <see cref="Get" /> unless it is evicted. If the new value is loaded successfully it will
        ///     replace the previous value in the cache; if an exception is thrown while refreshing the previous value will remain,
        ///     <em>and the exception will be logged and swallowed</em>.
        ///     <para>
        ///         Caches loaded by an <see cref="ICacheLoader{TKey,TValue}" /> will call
        ///         <see cref="ICacheLoader{TKey,TValue}.Reload" /> if the cache currently contains a value for
        ///         <paramref name="key" />, and <see cref="ICacheLoader{TKey,TValue}.Load" /> otherwise.
        ///     </para>
        /// </summary>
        [PublicAPI]
        void Referesh(TKey key);

    }
}
