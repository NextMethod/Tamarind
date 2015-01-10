using System;
using System.Linq;

namespace Tamarind.Cache
{
    /// <summary>
    ///     Accumulates statistics during the operation of a <see cref="Cache" /> for presentation by
    ///     <see cref="IStatsCounter" /> . This is solely intended for consumption by <see cref="Cache" /> implementors.
    /// </summary>
    public interface IStatsCounter
    {

        /// <summary>
        ///     Records the cache hits. Should be called when a cache request returns a cached value.
        /// </summary>
        /// <param name="count">The number of hits to record.</param>
        void RecordHits(int count);

        /// <summary>
        ///     Records cache misses. This should be called when a cache request returns a value that was
        ///     not found in the cache. This method should be called by the loading thread, as well as by
        ///     threads blocking on the load. Multiple concurrent calls to <see cref="ICache{TKey, TValue}" /> lookup methods with
        ///     the same key on an absent value should result in a single call to either
        ///     <see cref="RecordLoadSuccess" /> or <see cref="RecordLoadException" /> and multiple calls to this method,
        ///     despite all being served by the results of a single load operation.
        /// </summary>
        /// <param name="count">The number of misses to record.</param>
        void RecordMisses(int count);

        /// <summary>
        ///     Records the successful load of a new entry. This should be called when a cache request
        ///     causes an entry to be loaded, and the loading completes successfully. In contrast to
        ///     <see cref="RecordMisses" />, this method should only be called by the loading thread.
        /// </summary>
        /// <param name="loadTime">The number of nanoseconds spent computing or retrieving the new value.</param>
        void RecordLoadSuccess(long loadTime);

        /// <summary>
        ///     Records the failed load of a new entry. This should be called when a cache request
        ///     causes an entry to be loaded, but an exception is thrown while doing so. In contrast to
        ///     <see cref="RecordMisses" />, this method should only be called by the loading thread.
        /// </summary>
        /// <param name="loadTime">
        ///     The number of nanoseconds spent computing or retrieving the new value, prior to the exception
        ///     being thrown.
        /// </param>
        void RecordLoadException(long loadTime);

        /// <summary>
        ///     Records the eviction of an entry from the cache. This should only been called when an entry
        ///     is evicted due to the cache's eviction strategy, and not as a result of manual
        ///     <see cref="ICache{TKey, TValue}.Invalidate" /> invalidations.
        /// </summary>
        void RecordEviction();

        /// <summary>
        ///     Returns a snapshot of this counter's values. Note that this may be an inconsistent view, as
        ///     it may be interleaved with update operations.
        /// </summary>
        /// <returns></returns>
        ICacheStats Snapshot();

    }
}
