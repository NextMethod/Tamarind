using System;
using System.Linq;
using System.Threading;

namespace Tamarind.Cache
{
    public class SimpleStatsCounter : IStatsCounter
    {
        private long hitCount = 0;
        private long missCount = 0;
        private long loadSuccessCount = 0;
        private long loadExceptionCount = 0;
        private long totalLoadTime = 0;
        private long evictionCount = 0;

        public SimpleStatsCounter() {}

        public void RecordHits(int count)
        {
            Interlocked.Add(ref hitCount, count);
        }

        public void RecordMisses(int count)
        {
            Interlocked.Add(ref missCount, count);
        }

        public void RecordLoadSuccess(long loadTime)
        {
            Interlocked.Increment(ref loadSuccessCount);
            Interlocked.Add(ref totalLoadTime, loadTime);
        }

        public void RecordLoadException(long loadTime)
        {
            Interlocked.Increment(ref loadExceptionCount);
            Interlocked.Add(ref totalLoadTime, loadTime);
        }

        public void RecordEviction()
        {
            Interlocked.Increment(ref evictionCount);
        }

        public ICacheStats Snapshot()
        {
            return new CacheStats(hitCount, missCount, loadSuccessCount, loadExceptionCount, totalLoadTime, evictionCount);
        }
    }
}
