using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamarind.Cache
{
    /// <summary>
    /// This class provides a skeletal implementation of the <see cref="ICache"/> interface to minimize the
    /// effort required to implement this interface.
    /// </summary>
    public abstract class Cache<TKey, TValue> : ICache<TKey, TValue>
    {
        public long Count
        {
            get { throw new NotImplementedException(); }
        }

        public IObservable<RemovalNotification<TKey, TValue>> RemovalNotifier
        {
            get { throw new NotImplementedException(); }
        }

        public TValue GetIfPresent(TKey key)
        {
            throw new NotImplementedException();
        }

        public TValue Get(TKey key, Func<TValue> valueLoader)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyDictionary<TKey, TValue> GetAllPresent(IEnumerable<TKey> keys)
        {
            var presentPairs = new Dictionary<TKey, TValue>(keys.Count());
            foreach (var key in keys)
            {
                if (!presentPairs.ContainsKey(key))
                {
                    presentPairs.Add(key, GetIfPresent(key));
                }
            }

            return new ReadOnlyDictionary<TKey, TValue>(presentPairs);
        }

        public void Put(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public void PutAll(IDictionary<TKey, TValue> dictionary)
        {
            foreach (var entry in dictionary)
            {
                Put(entry.Key, entry.Value);
            }
        }

        public void Invalidate(TKey key)
        {
            throw new NotImplementedException();
        }

        public void InvlidateAll(IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                Invalidate(key);
            }
        }

        public ICacheStats Stats()
        {
            throw new NotImplementedException();
        }

        public ConcurrentDictionary<TKey, TValue> ToDictionary()
        {
            throw new NotImplementedException();
        }

        public void CleanUp()
        {
            throw new NotImplementedException();
        }
    }
}
