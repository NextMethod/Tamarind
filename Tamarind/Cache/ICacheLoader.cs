using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tamarind.Cache
{

    /// <summary>
    ///     Computes or retrieves values, based on a key, for use in populating a <see cref="ILoadingCache{TKey,TValue}" />.
    /// </summary>
    public interface ICacheLoader<TKey, TValue>
    {

        /// <summary>
        ///     Computes or retrieves the value corresponding to <paramref name="key" />
        /// </summary>
        /// <param name="key">the non-null key who's value should be loaded</param>
        TValue Load(TKey key);

        /// <summary>
        ///     Computes or retrieves the values corresponding to <paramref name="keys" />. This emthod is called by
        ///     <see cref="ILoadingCache{TKey,TValue}.GetAll" />.
        ///     <para>
        ///         If the returned dictionary doesn't contain all requested <paramref name="keys" /> then the entries it does
        ///         contain will be cached, but <see cref="ILoadingCache{TKey,TValue}.GetAll" /> will throw an exception. If the
        ///         returned map contains extra keys not present in <paramref name="keys" /> then all returned entries will be
        ///         cached, but only the entries for <paramref name="keys" /> will be returned from
        ///         <see cref="ILoadingCache{TKey,TValue}.GetAll" />
        ///     </para>
        /// </summary>
        IReadOnlyDictionary<TKey, TValue> LoadAll(IEnumerable<TKey> keys);

        /// <summary>
        ///     Computes or retrieves a replace value corresponding to an already-cached <paramref name="key" />. This method is
        ///     called when an
        ///     existing cache entry is refreshed by <see cref="CacheBuilder{TKey,TValue}.RefreshAfterWrite" />, or through a call
        ///     to <see cref="ILoadingCache{TKey,TValue}.Referesh" />.
        /// </summary>
        Task<TValue> Reload(TKey key, TValue oldValue);

    }
}
