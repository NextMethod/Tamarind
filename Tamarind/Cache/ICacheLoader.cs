using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tamarind.Cache
{
	public interface ICacheLoader<TKey, TValue>
	{

		TValue Load(TKey key);

		IReadOnlyDictionary<TKey, TValue> LoadAll(IEnumerable<TKey> keys);

		Task<TValue> Reload(TKey key, TValue oldValue);

	}
}
