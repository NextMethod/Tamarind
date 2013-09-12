using System;
using System.Linq;

namespace Tamarind.Cache
{
	// Guava reference: https://code.google.com/p/guava-libraries/source/browse/guava/src/com/google/common/cache/CacheStats.java
	/// <summary>
	///     Statistics about the performance of a <seealso cref="Cache.ICache" />. Instances of this class are immutable.
	/// </summary>
	public interface ICacheStats
	{

		double AverageLoadPenalty { get; }

		long EvictionCount { get; }

		long HitCount { get; }

		double HitRate { get; }

		long LoadCount { get; }

		long LoadExceptionCount { get; }

		double LoadExceptionRate { get; }

		long LoadSuccessCount { get; }

		long MissCount { get; }

		double MissRate { get; }

		long RequestCount { get; }

		long TotalLoadTime { get; }

		ICacheStats Minus(ICacheStats other);

		ICacheStats Plus(ICacheStats other);

	}
}
