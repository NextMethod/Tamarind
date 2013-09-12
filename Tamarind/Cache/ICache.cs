using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Tamarind.Cache
{
    // Guava Reference: https://code.google.com/p/guava-libraries/source/browse/guava/src/com/google/common/cache/Cache.java
    /// <summary>
    /// A semi-persistent mapping from keys to values. Cache entries are manually added using
    /// <see cref="Get"/> or <see cref="Put"/>, and are stored in the cache until
    /// either evicted or manually invalidated.
    /// 
    /// Implementations of this interface are expected to be thread-safe, and can be safely accessed
    /// by multiple concurrent threads.
    /// </summary>
    /// <typeparam name="K">The cache's key type.</typeparam>
    /// <typeparam name="V">The cache's value type.</typeparam>
    public interface ICache<K, V>
    {
        V GetIfPresent(K key);
        V Get(K key, Func<V> valueLoader);
        ImmutableDictionary<K, V> GetAllPresent(IEnumerator<K> keys);
        void Put(K key, V value);
        void PutAll(Dictionary<K, V> xs);
        void Invalidate(K key);
        void InvlidateAll(IEnumerator<K> keys);
        long Count { get; }
        //CacheStats Stats { get; }
        ConcurrentDictionary<K, V> ToDictionary();
        void CleanUp();
    }
}
