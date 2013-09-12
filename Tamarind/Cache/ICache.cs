using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Tamarind.Cache
{
    // Guava Reference: https://code.google.com/p/guava-libraries/source/browse/guava/src/com/google/common/cache/Cache.java
    // TODO: NEEDS DOCUMENTATION.
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
