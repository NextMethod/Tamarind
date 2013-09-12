using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Tamarind.Cache
{
	// Guava Reference: https://code.google.com/p/guava-libraries/source/browse/guava/src/com/google/common/cache/Cache.java
	/// <summary>
	///     A semi-persistent mapping from keys to values. Cache entries are manually added using
	///     <see cref="Get" /> or <see cref="Put" />, and are stored in the cache until
	///     either evicted or manually invalidated.
	///     Implementations of this interface are expected to be thread-safe, and can be safely accessed
	///     by multiple concurrent threads.
	/// </summary>
	/// <typeparam name="TKey">The cache's key type.</typeparam>
	/// <typeparam name="TValue">The cache's value type.</typeparam>
	public interface ICache<TKey, TValue>
	{

		long Count { get; }

		ICacheStats Stats { get; }

		TValue GetIfPresent(TKey key);

		TValue Get(TKey key, Func<TValue> valueLoader);

		IReadOnlyDictionary<TKey, TValue> GetAllPresent(IEnumerator<TKey> keys);

		void Put(TKey key, TValue value);

		void PutAll(IDictionary<TKey, TValue> xs);

		void Invalidate(TKey key);

		void InvlidateAll(IEnumerator<TKey> keys);

		ConcurrentDictionary<TKey, TValue> ToDictionary();

		void CleanUp();

	}
}
