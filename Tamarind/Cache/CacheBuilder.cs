using System;
using System.Diagnostics;
using System.Linq;

using Tamarind.Annotations;
using Tamarind.Base;

namespace Tamarind.Cache
{
    /// <summary>
    ///     A builder of <see cref="ILoadingCache{TKey, TValue}" /> and <see cref="ICache{TKey, TValue}" /> isntances having
    ///     any combination of the following features:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>automatic loading of entries into the cache</description>
    ///         </item>
    ///         <item>
    ///             <description>least-recently-used eviction when a maximum size is expected</description>
    ///         </item>
    ///         <item>
    ///             <description>time-based expiration of entries, measured since last access or last write</description>
    ///         </item>
    ///         <item>
    ///             <description>keys automatically wrapped in <see cref="WeakReference{T}" /> references</description>
    ///         </item>
    ///         <item>
    ///             <description>accumulation of cache access statistics</description>
    ///         </item>
    ///     </list>
    ///     <para>
    ///         These features are all optional; caches can be created using all or none of them. By default cache instances
    ///         created by <see cref="CacheBuilder{TKey,TValue}" /> will not perform any type of eviction.
    ///     </para>
    /// </summary>
    /// <example>
    ///     <code>
    ///  LoadingCache&lt;Key, Graph&gt; graphs = CacheBuilder.NewBuilder()
    /// 			.MaximumSize(10000)
    /// 			.ExpireAfterWrite(TimeSpan.FromMinutes(10))
    /// 			.Build(new MyGraphCacheLoader&lt;Key, Graph&gt;());
    ///  </code>
    /// </example>
    [PublicAPI]
    public sealed class CacheBuilder<TKey, TValue>
    {

        private CacheBuilder() {}

        internal long? ExpireAfterAccessTicks { get; private set; }

        internal long? ExpireAfterWriteTicks { get; private set; }

        internal int? InitialCapacityValue { get; private set; }

        internal long? MaximumSizeValue { get; private set; }

        internal long? MaximumWeightValue { get; private set; }

        internal bool? RecordStatsValue { get; private set; }

        internal long? RefreshAfterWriteTicks { get; private set; }

        internal IObserver<RemovalNotification<TKey, TValue>> RemovalObserver { get; private set; }

        internal Ticker TickerValue { get; private set; }

        internal bool? WeakKeysValue { get; private set; }

        internal bool? WeakValuesValue { get; private set; }

        internal IWeigher<TKey, TValue> WeigherValue { get; private set; }

        /// <summary>
        ///     Builds a cache which does not automatically load values when keys are requested.
        ///     <para>
        ///         Consider a <see cref="Build(ICacheLoader{TKey,TValue})" /> instead if it is feasible to implement a
        ///         <see cref="ICacheLoader{TKey,TValue}" />.
        ///     </para>
        ///     <para>
        ///         This method does not alter the state of this <see cref="CacheBuilder{TKey,TValue}" /> instance, so it can be
        ///         invoked again to create multiple independent caches.
        ///     </para>
        /// </summary>
        public ICache<TKey, TValue> Build()
        {
            CheckWeightWithWeigher();
            CheckNonLoadingCache();
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Builds a cache, which either returns an already-loaded value for a given key or atomically computes or retrieves it
        ///     using the supplied <see cref="ICacheLoader{TKey,TValue}" />. If another thread is currently loading the value for
        ///     this key, the cache simply waits for that thread to finish and returns its loaded value. Note that multiple threads
        ///     can concurrently load values for distinct keys.
        ///     <para>
        ///         This method does not alter the state of this <see cref="CacheBuilder{TKey,TValue}" /> instance, so it can be
        ///         invoked again to create multiple independent caches.
        ///     </para>
        /// </summary>
        public ILoadingCache<TKey, TValue> Build(ICacheLoader<TKey, TValue> loader)
        {
            CheckWeightWithWeigher();
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Specifies that each entry should be automatically removed from the cache and once a fixed duration has elapsed
        ///     after the entry's creation, the most recent replacement of its value, or its last access. Access time is reset by
        ///     all cache read and write operations (including <c>Cache.ToDictionary().Get(TKey)</c> and
        ///     <c>Cache.ToDictionary().Put(TKey, TValue)</c>, but not by operations on the collection-views of
        ///     <see cref="ICache{TKey,TValue}.ToDictionary" />.
        ///     <para>
        ///         When <paramref name="duration" /> is zero, this method hands off to <see cref="TimeSpan.MaxValue" />,
        ///         ignoring any otherwise-specified maximum size or weight.
        ///     </para>
        ///     <para>
        ///         Expired entries may be counted in <see cref="ICache{TKey,TValue}.Count" /> but will never be visible to read
        ///         or write operations. Expired entries are cleaned up as part of the routine maintenance described in the class
        ///         documentation.
        ///     </para>
        /// </summary>
        public CacheBuilder<TKey, TValue> ExpireAfterAccess(TimeSpan duration)
        {
            Preconditions.CheckState(ExpireAfterAccessTicks == null, "ExpireAfterAccess was aready set to {0} ticks", ExpireAfterAccessTicks);
            Preconditions.CheckArgument(duration.Ticks >= 0, "duration", "duration cannot be nagative: {0}", duration);

            ExpireAfterAccessTicks = duration.Ticks;

            return this;
        }

        /// <summary>
        ///     Specifies that each entry should be automatically removed from the cache and once a fixed duration has elapsed
        ///     after the entry's creation or the most recent replacement of its value.
        ///     <para>
        ///         When <paramref name="duration" /> is zero, this method hands off to <see cref="TimeSpan.MaxValue" />,
        ///         ignoring any otherwise-specified maximum size or weight.
        ///     </para>
        ///     <para>
        ///         Expired entries may be counted in <see cref="ICache{TKey,TValue}.Count" /> but will never be visible to read
        ///         or write operations. Expired entries are cleaned up as part of the routine maintenance described in the class
        ///         documentation.
        ///     </para>
        /// </summary>
        public CacheBuilder<TKey, TValue> ExpireAfterWrite(TimeSpan duration)
        {
            Preconditions.CheckState(ExpireAfterWriteTicks == null, "ExpireAfterWrite was aready set to {0} ticks", ExpireAfterWriteTicks);
            Preconditions.CheckArgument(duration.Ticks >= 0, "duration", "duration cannot be nagative: {0}", duration);

            ExpireAfterWriteTicks = duration.Ticks;
            return this;
        }

        public CacheBuilder<TKey, TValue> InitialCapacity(int initialCapacity)
        {
            Preconditions.CheckState(InitialCapacityValue == null, "InitialCapacity was aready set to {0}", InitialCapacityValue);
            Preconditions.CheckArgument(initialCapacity >= 0, "initialCapacity");

            InitialCapacityValue = initialCapacity;

            return this;
        }

        /// <summary>
        ///     Specifies the maximum number of entries the cache my contain.
        ///     <para>
        ///         Note that the cache
        ///         <strong>may evict an entry before this limit is exceeded</strong>. As the cache size grows close to the
        ///         maximum,
        ///         the cache evicts entries that are less likely to be used again. For example, the cache my evict an entry
        ///         because it
        ///         hasn't been used recently or very often.
        ///     </para>
        ///     <para>
        ///         When <paramref name="size" /> is zero, entries will be evicted immediately after being loaded into the cache.
        ///         This can be useful in testing.
        ///     </para>
        ///     <para>This feature cannot be used in conjunction with <see cref="MaximumWeight" />.</para>
        /// </summary>
        public CacheBuilder<TKey, TValue> MaximumSize(long size)
        {
            Preconditions.CheckState(MaximumSizeValue == null, "MaximumSize was aready set to {0}", MaximumSizeValue);
            Preconditions.CheckState(MaximumWeightValue == null, "MaximumWeight was aready set to {0}", MaximumWeightValue);
            Preconditions.CheckState(WeigherValue == null, "MaximumSize can not be combined with Weigher");
            Preconditions.CheckArgument(size >= 0, "size", "MaximumSize must not be negative");

            MaximumSizeValue = size;

            return this;
        }

        /// <summary>
        ///     Specifies the maximum weight of entries the cache may contain. Weight is determined using the
        ///     <see cref="IWeigher{TKey,TValue}" /> specified with <see cref="Weigher" />, and use of this method requires a
        ///     corresponding call to <see cref="Weigher" /> prior to calling <see cref="Build()" /> or
        ///     <see cref="Build(ICacheLoader{TKey,TValue})" />.
        ///     <para>
        ///         Note that the cache
        ///         <strong>may evict an entry before this limit is exceeded</strong>. As the cache size grows close to the
        ///         maximum,
        ///         the cache evicts entries that are less likely to be used again. For example, the cache my evict an entry
        ///         because it
        ///         hasn't been used recently or very often.
        ///     </para>
        ///     <para>
        ///         When <paramref name="weight" /> is zero, entries will be evicted immediately after being loaded into the cache.
        ///         This can be useful in testing.
        ///     </para>
        ///     <para>
        ///         Note that weight is only used to determine whether the cache is over capacity; it has no effect on selecting
        ///         which entry should be evicted next.
        ///     </para>
        ///     <para>This feature cannot be used in conjunction with <see cref="MaximumSize" />.</para>
        /// </summary>
        public CacheBuilder<TKey, TValue> MaximumWeight(long weight)
        {
            Preconditions.CheckState(MaximumWeightValue == null, "MaximumWeight was aready set to {0}", MaximumWeightValue);
            Preconditions.CheckState(MaximumSizeValue == null, "MaximumSize was aready set to {0}", MaximumSizeValue);
            Preconditions.CheckArgument(weight >= 0, "weight", "MaximumWeight must not be negative");

            MaximumWeightValue = weight;

            return this;
        }

        /// <summary>
        ///     Enable the accumulation of <see cref="ICacheStats" /> during the operation of the cache. Without this
        ///     <see cref="ICache{TKey,TValue}.Stats" /> will return zero for all statistics.
        ///     <para>
        ///         <strong>Note</strong> that recording stats requires bookkeeping to be performed with each operation, and thus
        ///         imposes a performance penalty on cache operation.
        ///     </para>
        /// </summary>
        public CacheBuilder<TKey, TValue> RecordStats()
        {
            RecordStatsValue = true;
            return this;
        }

        /// <summary>
        ///     Specifies that active entries are eligible for automatic refresh once a fixed duration has elapsed after the
        ///     entry's creation, or the most recent replacement of its value. The semantics of refreshes are specified in
        ///     <see cref="ILoadingCache{TKey,TValue}.Referesh" />, and are perfomed by calling
        ///     <see cref="ICacheLoader{TKey,TValue}.Reload" />.
        /// </summary>
        public CacheBuilder<TKey, TValue> RefreshAfterWrite(TimeSpan duration)
        {
            Preconditions.CheckState(RefreshAfterWriteTicks == null, "RefreshAfterWrite was aready set to {0} ticks", RefreshAfterWriteTicks);
            Preconditions.CheckArgument(duration.Ticks >= 0, "duration", "duration cannot be nagative: {0}", duration);

            RefreshAfterWriteTicks = duration.Ticks;

            return this;
        }

        /// <summary>
        ///     Specifies a tick-precision time source for use in determining when entries should be expired. By default,
        ///     <c>DateTime.Now.Ticks</c> is used.
        ///     <param>
        ///         The primary intent of this method is to facilitate testing of caches which have been configured with
        ///         <see cref="ExpireAfterAccess" /> or <see cref="ExpireAfterWrite" />.
        ///     </param>
        /// </summary>
        public CacheBuilder<TKey, TValue> Ticker(Ticker ticker)
        {
            Preconditions.CheckState(TickerValue == null, "Ticker was aready set");

            TickerValue = Preconditions.CheckNotNull(ticker);

            return this;
        }

        /// <summary>
        ///     Specifies that each key (not value) stored in the cache should be wrapped in a <see cref="WeakReference{T}" /> (by
        ///     default strong references are used).
        ///     <para>
        ///         Entries with keys that have been garbage collected may be counted in <see cref="ICache{TKey,TValue}.Count" />
        ///         , but will never be visible to read or write operations; such entries are cleaned up as part of the routing
        ///         maintenance described in the class documentation.
        ///     </para>
        /// </summary>
        public CacheBuilder<TKey, TValue> WeakKeys()
        {
            Preconditions.CheckState(WeakKeysValue == null, "WeakKeys was aready set");

            WeakKeysValue = true;

            return this;
        }

        /// <summary>
        ///     Specifies that each value (not key) stored in the cache should be wrapped in a <see cref="WeakReference{T}" /> (by
        ///     default strong references are used).
        ///     <para>
        ///         Weak values will be garbage collected once they are weakly reachable. This makes them a poor candidate for
        ///         caching.
        ///     </para>
        ///     <para>
        ///         Entries with values that have been garbage collected may be counted in <see cref="ICache{TKey,TValue}.Count" />
        ///         , but will never be visible to read or write operations; such entries are cleaned up as part of the routing
        ///         maintenance described in the class documentation.
        ///     </para>
        /// </summary>
        public CacheBuilder<TKey, TValue> WeakValues()
        {
            Preconditions.CheckState(WeakValuesValue == null, "WeakValues was aready set");

            WeakValuesValue = true;

            return this;
        }

        /// <summary>
        ///     Specifies the weigher to use in determining the weight of entries. Entry weight is taken into consideration by
        ///     <see cref="MaximumWeight" /> when determining which entries to evict, and use of this method requires a
        ///     corresponding call to <see cref="MaximumWeight" /> prior to calling <see cref="Build()" /> or
        ///     <see cref="Build(ICacheLoader{TKey,TValue})" />. Weights are measured and recorded when entries are inserted into
        ///     the cache, and are thus effectively static during the lifetime of a cache entry.
        ///     <para>
        ///         When the weight of an entry is zero it will not be considered for size-based eviction (though it still may be
        ///         evicted by other means).
        ///     </para>
        /// </summary>
        public CacheBuilder<TKey, TValue> Weigher(IWeigher<TKey, TValue> weigher)
        {
            Preconditions.CheckState(WeigherValue == null, "Weigher was already set");

            WeigherValue = weigher;

            return this;
        }

        private void CheckWeightWithWeigher()
        {
            if (WeigherValue == null)
            {
                Preconditions.CheckState(MaximumWeightValue == null, "MaximumWeight requires weigher");
            }
            else if (MaximumWeightValue == null)
            {
                Debug.WriteLine("Ignoring Weigher specified without MaximumWeight");
            }
        }

        private void CheckNonLoadingCache()
        {
            Preconditions.CheckState(RefreshAfterWriteTicks == null, "RefreshAfterWrite requires an ILoadingCache");
        }

        /// <summary>
        ///     Constructs a new <see cref="CacheBuilder{TKey,TValue}" /> instance with default settings, including stong keys,
        ///     strong values, and no automatic eviction of any kind.
        /// </summary>
        public static CacheBuilder<TCacheKey, TCacheValue> NewBuilder<TCacheKey, TCacheValue>()
        {
            return new CacheBuilder<TCacheKey, TCacheValue>();
        }

    }
}
