using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamarind.Cache
{
    public abstract class LoadingCache<TKey, TValue> : Cache<TKey, TValue>, ILoadingCache<TKey, TValue>
    {
        public TValue Get(TKey key)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyDictionary<TKey, TValue> GetAll(IEnumerable<TKey> keys)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach(var key in keys)
            {
                if (!result.ContainsKey(key))
                {
                    result.Add(key, Get(key));
                }
            }
            return new ReadOnlyDictionary<TKey, TValue>(result);
        }

        public void Referesh(TKey key)
        {
            throw new NotImplementedException();
        }
    }
}
