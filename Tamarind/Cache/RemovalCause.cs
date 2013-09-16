using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Tamarind.Cache
{
	/// <summary>
	///     The reason why a cached entry was removed.
	/// </summary>
	public enum RemovalCause
	{

		/// <summary>
		///     The entry was removed automatically because its key or value was garbage-collected. This can occur when using
		///     <see cref="CacheBuilder{TKey,TValue}.WeakKeys" /> or <see cref="CacheBuilder{TKey,TValue}.WeakValues" />.
		/// </summary>
		Collected,

		/// <summary>
		///     The entry's expiration timestamp has passed. This can occur when using
		///     <see cref="CacheBuilder{TKey,TValue}.ExpireAfterAccess" /> or
		///     <see cref="CacheBuilder{TKey,TValue}.ExpireAfterWrite" />.
		/// </summary>
		Expired,

		/// <summary>
		///     The entry was manually removed by the user. This can result from the user invoking
		///     <see cref="ICache{TKey,TValue}.Invalidate" />, <see cref="ICache{TKey,TValue}.InvlidateAll" />,
		///     <see cref="ConcurrentDictionary{TKey,TValue}.Remove(TKey)" />, or
		///     <see cref="ConcurrentDictionary{TKey,TValue}.TryRemove" />.
		/// </summary>
		Explicit,

		/// <summary>
		///     The entry itself was not actually removed, but its value was replaced by the user. This can result from the user
		///     invoking <see cref="ICache{TKey,TValue}.Put" />, <see cref="ILoadingCache{TKey,TValue}.Referesh" />,
		///     <see cref="ICache{TKey,TValue}.PutAll" />,
		///     <see cref="ConcurrentDictionary{TKey,TValue}.AddOrUpdate(TKey,System.Func{TKey,TValue},System.Func{TKey,TValue,TValue})" />
		///     , or <see cref="ConcurrentDictionary{TKey,TValue}.TryUpdate" />
		/// </summary>
		Replaced,

		/// <summary>
		///     The entry was evicted due to size constraints. This can occur when using
		///     <see cref="CacheBuilder{TKey,TValue}.MaximumSize" /> or <see cref="CacheBuilder{TKey,TValue}.MaximumWeight" />.
		/// </summary>
		Size

	}
}
