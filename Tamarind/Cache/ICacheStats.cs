using System;
using System.Linq;

using Tamarind.Annotations;

namespace Tamarind.Cache
{
	/// <summary>
	///     Statistics about the performance of a <seealso cref="ICache{TKey,TValue}" />. Instances of this class are
	///     immutable.
	///     <para>Cache statistics are incremented according to the following rules:</para>
	///     <list type="bullet">
	///         <item>
	///             <description>When a cache lookup encounters an existing cache entry <c>HitCount</c> is incremented.</description>
	///         </item>
	///         <item>
	///             <description>
	///                 When a cache lookup first encounters a missing cache entry, a new entry is loaded.
	///                 <list type="bullet">
	///                     <item>
	///                         <description>
	///                             After successfully loading an entry <c>MissCount</c> and <c>LoadSuccessCount</c>
	///                             are incremented, and the total loading time, in nanoseconds, is added to
	///                             <c>TotalLoadTime</c>
	///                         </description>
	///                     </item>
	///                     <item>
	///                         <description>
	///                             When an exception is thrown while loading an entry, <c>MissCount</c> and
	///                             <c>LoadExceptionCount</c> are incremented, and the total loading time, in nanoseconds, is
	///                             added to <c>TotalLoadTime</c>
	///                         </description>
	///                     </item>
	///                     <item>
	///                         <description>
	///                             Cache lookups that encounter a missing cache entry that is still loading will wait
	///                             for loading to complete (whether successful or not) and then increment <c>MissCount</c>.
	///                         </description>
	///                     </item>
	///                 </list>
	///             </description>
	///         </item>
	///         <item>
	///             <description>When an etry is evicted from the cache, <c>EvictionCount</c> is incremented.</description>
	///         </item>
	///         <item>
	///             <description>No stats are modified when a cache entry is invalidated or manually removed.</description>
	///         </item>
	///         <item>
	///             <description>No stats are modified on a query to <see cref="ICache{TKey,TValue}.GetIfPresent" />.</description>
	///         </item>
	///         <item>
	///             <description>
	///                 No stats are modified by operations invoked on the
	///                 <see cref="ICache{TKey,TValue}.ToDictionary" /> view of the cache.
	///             </description>
	///         </item>
	///     </list>
	/// </summary>
	/// <remarks>
	///     <para>
	///         See
	///         <a href="http://docs.guava-libraries.googlecode.com/git-history/release/javadoc/com/google/common/cache/CacheStats.html">
	///             Guava
	///             Reference
	///         </a>
	///     </para>
	/// </remarks>
	public interface ICacheStats
	{

		/// <summary>
		///     The average time spent loading new values. This is defined as
		///     <c>TotalLoadTime / (LoadSuccessCount + LoadExceptionCount)</c>.
		/// </summary>
		[PublicAPI]
		double AverageLoadPenalty { get; }

		/// <summary>
		///     The number of times an entry has been evicted. This count does not include manual
		///     <see cref="ICache{TKey,TValue}.Invalidate">invalidations</see>.
		/// </summary>
		[PublicAPI]
		long EvictionCount { get; }

		/// <summary>
		///     The number of times <see cref="ICache{TKey,TValue}" /> lookup methods have returned a cached value.
		/// </summary>
		[PublicAPI]
		long HitCount { get; }

		/// <summary>
		///     The ratio of cache requests which were hits. This si defined as <c>HitCount / RequestCount</c>, or
		///     <c>1.0</c> when <c>RequestCount == 0</c>.
		/// </summary>
		/// <remarks>
		///     Note that <c>HitRate + MissRate =~ 1.0</c>
		/// </remarks>
		[PublicAPI]
		double HitRate { get; }

		/// <summary>
		///     The total number of times that <see cref="ICache{TKey,TValue}" /> lookup methods attempted to load new values. This
		///     includes both successful load operations, as well as those that threw exceptions. This is defined as
		///     <c>LoadSuccessCount + LoadExceptionCount</c>.
		/// </summary>
		[PublicAPI]
		long LoadCount { get; }

		/// <summary>
		///     The number of times <see cref="ICache{TKey,TValue}" /> lookup methods threw an exception while loading a new value.
		///     This is always incremented in conjunction with <see cref="MissCount" />, though <see cref="MissCount" /> is also
		///     incremented when cache loading completes successfully (see <see cref="LoadExceptionCount" />). Multiple concurrent
		///     misses for the same key will result in a single load operation.
		/// </summary>
		[PublicAPI]
		long LoadExceptionCount { get; }

		/// <summary>
		///     The ratio of cache loading attempts which threw exceptions. This is defined as
		///     <c>LoadExceptionCount / (LoadSuccessCount + LoadExceptionCount)</c> or <c>0.0</c> when
		///     <c>LoadSuccessCount + LoadExceptionCount == 0</c>.
		/// </summary>
		[PublicAPI]
		double LoadExceptionRate { get; }

		/// <summary>
		///     The number of times <see cref="ICache{TKey,TValue}" /> lookup methods have successfully loaded a new value. This is
		///     always incremented in conjunction with <see cref="MissCount" />, though <c>MissCount</c> is also incremented when
		///     an exception is encountered during cache loading (see <see cref="LoadExceptionCount" />). Multiple concurrent
		///     misses for the same key will result in a single load operation.
		/// </summary>
		[PublicAPI]
		long LoadSuccessCount { get; }

		/// <summary>
		///     The number of times <see cref="ICache{TKey,TValue}" /> lookup methods have returned an uncached (newly
		///     loaded) value, or null. Multiple concurrent calls to <see cref="ICache{TKey,TValue}" /> lookup methods on an absent
		///     value can result in multiple misses, all returning the results of a single cache load operation.
		/// </summary>
		[PublicAPI]
		long MissCount { get; }

		/// <summary>
		///     The ratio of cache requests which were misses. This is defined as <c>MissCount / RequestCount</c>, or <c>0.0</c>
		///     when <c>RequestCount == 0</c>.
		/// </summary>
		/// <remarks>
		///     Note that <c>HitRate + MissRate =~ 1.0</c>. Cache misses include all requests which weren't cache hits, includeing
		///     requests which resulted in either successful or failed loading attempts, and requests which waited for other
		///     threads to finish loading. It is thus the case that <c>MissCount >= LoadSuccessCount + LoadExceptionCount</c>.
		///     Multiple concurrent misses for the same key will result in a single load operation.
		/// </remarks>
		[PublicAPI]
		double MissRate { get; }

		/// <summary>
		///     The number of times <see cref="ICache{TKey,TValue}" /> lookup methods have returned either a cached or
		///     uncached
		///     value.
		///     This is defined as <c>HitCount + MissCount</c>.
		/// </summary>
		[PublicAPI]
		long RequestCount { get; }

		/// <summary>
		///     The total number of nanoseconds the cache has spent loading new values. This can be used to calculate the miss
		///     penalty. This value is increased every time <c>LoadSuccessCount</c> or <c>LoadExceptionCount</c> is incremented.
		/// </summary>
		[PublicAPI]
		long TotalLoadTime { get; }

		/// <summary>
		///     Returns a new <see cref="ICacheStats" /> representing the difference between this
		///     <see cref="ICacheStats" /> and <paramref name="other" />. Negative values, which aren't supported by
		///     <see cref="ICacheStats" /> will be rounded up to zero.
		/// </summary>
		[PublicAPI]
		ICacheStats Minus(ICacheStats other);

		/// <summary>
		///     Returns a new <see cref="ICacheStats" /> representing the sum of this <see cref="ICacheStats" /> and
		///     <paramref name="other" />.
		/// </summary>
		[PublicAPI]
		ICacheStats Plus(ICacheStats other);

	}
}
