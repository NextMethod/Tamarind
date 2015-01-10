using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tamarind.Cache
{
    /// <summary>
    ///     This class provides a skeletal implementation of the {@code Cache} interface to minimize the
    ///     effort required to implement this interface.
    ///     <para>
    ///         To implement a cache, the programmer needs only to extend this class and provide an
    ///         implementation for the <see cref="Get" /> and <see cref="getIfPresent" /> methods.
    ///         <see cref="Get(TKey, Func
    ///         <TValue>
    ///             )"/>, and <see cref="GetAll" /> are implemented in
    ///             terms of <see cref="Get" />; <see cref="GetAllPresent" /> is implemented in terms of
    ///             <see cref="GetIfPresent" />;
    ///             <see cref="PutAll" /> is implemented in terms of <see cref="Put" />,
    ///             <see cref="InvalidateAll(IEnumerable)" /> is
    ///             implemented in terms of <see cref="Invalidate" />. The method <see cref="CleanUp" /> is a no-op. All other
    ///             methods throw an <see cref="NotImplementedException" />.
    ///     </para>
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public abstract class LoadingCache<TKey, TValue> : Cache<TKey, TValue>, ILoadingCache<TKey, TValue>
    {

        public TValue Get(TKey key)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyDictionary<TKey, TValue> GetAll(IEnumerable<TKey> keys)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var key in keys)
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
