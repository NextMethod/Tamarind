using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamarind.Cache
{
    // TODO: NEEDS DOCUMENTATION.
    public interface ICache<K, V>
    {
        public V GetIfPresent(K key);
        public V Get(K key, Func<V> valueLoader);
        public ImmutableDictionary<K, V> GetAllPresent(IEnumerator<K> keys);
        public void Put(K key, V value);
        public void PutAll(Dictionary<K, V> xs);
        public void Invalidate(K key);
        public void InvlidateAll(IEnumerator<K> keys);
        public long Count { get; }
        //public CacheStats Stats { get; }
        ConcurrentDictionary<K, V> ToDictionary();
        public void CleanUp();
    }
}
