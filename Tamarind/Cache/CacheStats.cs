using System;
using System.Linq;

using Tamarind.Base;

namespace Tamarind.Cache
{
	internal class CacheStats : ICacheStats
	{

		public CacheStats(long hitCount, long missCount, long loadSuccessCount, long loadExceptionCount, long totalLoadTime, long evictionCount)
		{
			Preconditions.CheckArgument(hitCount >= 0);
			Preconditions.CheckArgument(missCount >= 0);
			Preconditions.CheckArgument(loadSuccessCount >= 0);
			Preconditions.CheckArgument(loadExceptionCount >= 0);
			Preconditions.CheckArgument(totalLoadTime >= 0);
			Preconditions.CheckArgument(evictionCount >= 0);

			HitCount = hitCount;
			MissCount = missCount;
			LoadSuccessCount = loadSuccessCount;
			LoadExceptionCount = loadExceptionCount;
			TotalLoadTime = totalLoadTime;
			EvictionCount = evictionCount;

			#region Calculated Properties

			LoadCount = loadSuccessCount + loadExceptionCount;

			RequestCount = hitCount + missCount;

			HitRate = RequestCount == 0 ? 1.0 : (double) hitCount / RequestCount;

			MissRate = RequestCount == 0 ? 0.0 : (double) missCount / RequestCount;

			LoadExceptionRate = LoadCount == 0 ? 0.0 : (double) loadExceptionCount / LoadCount;

			AverageLoadPenalty = LoadCount == 0 ? 0.0 : (double) totalLoadTime / LoadCount;

			#endregion
		}

		public double AverageLoadPenalty { get; private set; }

		public long EvictionCount { get; private set; }

		public long HitCount { get; private set; }

		public double HitRate { get; private set; }

		public long LoadCount { get; private set; }

		public long LoadExceptionCount { get; private set; }

		public double LoadExceptionRate { get; private set; }

		public long LoadSuccessCount { get; private set; }

		public long MissCount { get; private set; }

		public double MissRate { get; private set; }

		public long RequestCount { get; private set; }

		public long TotalLoadTime { get; private set; }

		public ICacheStats Minus(ICacheStats other)
		{
			return new CacheStats(
				System.Math.Max(0, HitCount - other.HitCount),
				System.Math.Max(0, MissCount - other.MissCount),
				System.Math.Max(0, LoadSuccessCount - other.LoadSuccessCount),
				System.Math.Max(0, LoadExceptionCount - other.LoadExceptionCount),
				System.Math.Max(0, TotalLoadTime - other.TotalLoadTime),
				System.Math.Max(0, EvictionCount - other.EvictionCount)
				);
		}

		public ICacheStats Plus(ICacheStats other)
		{
			return new CacheStats(
				HitCount + other.HitCount,
				MissCount + other.MissCount,
				LoadSuccessCount + other.LoadSuccessCount,
				LoadExceptionCount + other.LoadExceptionCount,
				TotalLoadTime + other.TotalLoadTime,
				EvictionCount + other.EvictionCount
				);
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			hashCode = ((hashCode << 5) + hashCode) ^ HitCount.GetHashCode();
			hashCode = ((hashCode << 5) + hashCode) ^ MissCount.GetHashCode();
			hashCode = ((hashCode << 5) + hashCode) ^ LoadSuccessCount.GetHashCode();
			hashCode = ((hashCode << 5) + hashCode) ^ LoadExceptionCount.GetHashCode();
			hashCode = ((hashCode << 5) + hashCode) ^ TotalLoadTime.GetHashCode();
			hashCode = ((hashCode << 5) + hashCode) ^ EvictionCount.GetHashCode();

			return hashCode;
		}

		public override bool Equals(object obj)
		{
			if (obj == null) { return false; }

			var other = obj as ICacheStats;
			if (other == null) { return false; }

			return HitCount == other.HitCount
					&& MissCount == other.MissCount
					&& LoadSuccessCount == other.LoadSuccessCount
					&& LoadExceptionCount == other.LoadExceptionCount
					&& TotalLoadTime == other.TotalLoadTime
					&& EvictionCount == other.EvictionCount;
		}

		public override string ToString()
		{
			return string.Format(
				"HitCount: {0}, MissCount: {1}, LoadSuccessCount: {2}, LoadExceptionCount: {3}, TotalLoadTime: {4}, EvictionCount: {5}",
				HitCount,
				MissCount,
				LoadSuccessCount,
				LoadExceptionCount,
				TotalLoadTime,
				EvictionCount);
		}

	}
}
