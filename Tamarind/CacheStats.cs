using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamarind
{
    // Guava reference: https://code.google.com/p/guava-libraries/source/browse/guava/src/com/google/common/cache/CacheStats.java
    /// <summary>
    /// Statistics about the performance of a <seealso cref="Cache.ICache"/>. Instances of this class are immutable.
    /// </summary>
    public sealed class CacheStats
    {
        private readonly long hitCount;
        private readonly long missCount;
        private readonly long loadSuccessCount;
        private readonly long loadExceptionCount;
        private readonly long totalLoadTime;
        private readonly long evictionCount;

        public CacheStats(long hitCount, long missCount, long loadSucessCount, long loadExceptionCount,
            long totalLoadCount, long evictionCount)
        {
        }
    }
}
