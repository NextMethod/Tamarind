using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Tamarind.Annotations;

namespace Tamarind.Cache
{
	/// <summary>
	///     A semi-persistent mapping from keys to values. Cache entries are manually added using
	///     <see cref="Get" /> or <see cref="Put" />, and are stored in the cache until
	///     either evicted or manually invalidated.
	///     <para>
	///         Implementations of this interface are expected to be thread-safe, and can be safely accessed
	///         by multiple concurrent threads.
	///     </para>
	///     <para>
	///         See
	///         <a href="http://docs.guava-libraries.googlecode.com/git-history/release/javadoc/com/google/common/cache/Cache.html">
	///             Guava
	///             Reference
	///         </a>
	///     </para>
	/// </summary>
	/// <typeparam name="TKey">The cache's key type.</typeparam>
	/// <typeparam name="TValue">The cache's value type.</typeparam>
	public interface ICache<TKey, TValue>
	{

		/// <summary>
		///     The approximate number of entries in this cache.
		/// </summary>
		[PublicAPI]
		long Count { get; }

		/// <summary>
		///     Returns the value associated with <paramref name="key" /> in this cache, or <c>null</c> if there is no cached value
		///     for <paramref name="key" />
		/// </summary>
		[PublicAPI]
		TValue GetIfPresent(TKey key);

		/// <summary>
		///     Returns the value associated with <paramref name="key" /> in this cache, obtaining that value from
		///     <paramref name="valueLoader" /> if necessary. No observable state associated with this cache is modified until
		///     loading completes. This method provides a simple substitute for the conventional "if cached, return; otherwise
		///     create, cache and return" pattern.
		///     <para>
		///         <strong>Warning:</strong> <paramref name="valueLoader" /> <strong>must not</strong> return <c>null</c>; it
		///         may either return a non-null value or throw an exception.
		///     </para>
		/// </summary>
		[UsedImplicitly]
		TValue Get(TKey key, Func<TValue> valueLoader);

		/// <summary>
		///     Returns a map of the values associated with <paramref name="keys" /> in this cache. The returned map will only
		///     contain entries which are already present in the cache.
		/// </summary>
		[PublicAPI]
		IReadOnlyDictionary<TKey, TValue> GetAllPresent(IEnumerator<TKey> keys);

		/// <summary>
		///     Associates <paramref name="key" /> with <paramref name="value" /> in this cache. If the cache previously contained
		///     a value associated with <paramref name="key" />, the old value is replaced by <paramref name="value" />.
		/// </summary>
		/// <remarks>
		///     Prefer <see cref="Get" /> when using the conventional "if cached, return; otherwise create, cache and return"
		///     pattern.
		/// </remarks>
		[PublicAPI]
		void Put(TKey key, TValue value);

		/// <summary>
		///     Copies all of the mappings from the specified map to the cache. The effect of this call is equivalent to that of
		///     calling <see cref="Put" /> on this map once for each mapping from key <typeparamref name="TKey" /> to value
		///     <typeparamref name="TValue" /> in the specified map. The behavior of this operation is undefined if the specified
		///     map is modified while the operation is in progress.
		/// </summary>
		[PublicAPI]
		void PutAll(IDictionary<TKey, TValue> map);

		/// <summary>
		///     Discards any cached value for key <paramref name="key" />.
		/// </summary>
		[PublicAPI]
		void Invalidate(TKey key);

		/// <summary>
		///     Discards any cached value for keys <paramref name="keys" />.
		/// </summary>
		[PublicAPI]
		void InvlidateAll(IEnumerator<TKey> keys);

		/// <summary>
		///     Returns a current snapshot of this cache's cummulative statistics. All stats are initialized to zero, and are
		///     monotonically increasing over the lifetime of the cache.
		/// </summary>
		[PublicAPI]
		ICacheStats Stats();

		/// <summary>
		///     Returns a view of the entries stored in this cache as a thread-safe map. Modifications made to the map directly
		///     affect the cache.
		/// </summary>
		[PublicAPI]
		ConcurrentDictionary<TKey, TValue> ToDictionary();

		/// <summary>
		///     Performs any pending maintenance operations needed by the cache. Exactly which activities are performed
		///     <strong>
		///         <em>if any</em>
		///     </strong>
		///     is implementation-dependent.
		/// </summary>
		[PublicAPI]
		void CleanUp();

	}
}
