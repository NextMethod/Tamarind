using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamarind.Cache
{
    /// <summary>
    ///     An object that can receive a notification when an entry is removed from a cache. The removal
    ///     resulting in notification could have occured to an entry being manually removed or replaced, or
    ///     due to eviction resulting from timed expiration, exceeding a maximum size, or garbage
    ///     collection.
    ///     <para>
    ///         This class holds strong references to the key and value, regardless of the type of references the cache may
    ///         be using.
    ///     </para>
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface IRemovalListener<TKey, TValue>
    {
        /// <summary>
        /// Notifies the listener that a removel has occured (at some point in the past).
        /// </summary>
        /// <param name="notification">The cache item removal notification.</param>
        void onRemoval(RemovalNotification<TKey, TValue> notification);
    }
}
