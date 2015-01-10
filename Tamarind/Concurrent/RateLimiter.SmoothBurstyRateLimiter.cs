using System;
using System.Linq;

namespace Tamarind.Concurrent
{
    internal sealed class SmoothBurstyRateLimiter : SmoothRateLimiter
    {

        private readonly double maxBurstSeconds;

        public SmoothBurstyRateLimiter(SleepingStopwatch stopwatch, double maxBurstSec) : base(stopwatch)
        {
            maxBurstSeconds = maxBurstSec;
        }

        protected override void SetRate(double permitsPerSecond, double stableIntervalMicroseconds)
        {
            var oldMaxPermits = MaxPermits;
            MaxPermits = maxBurstSeconds * permitsPerSecond;

            if (double.IsPositiveInfinity(oldMaxPermits))
            {
                StoredPermits = MaxPermits;
            }
            else
            {
                StoredPermits = ((decimal) 0.0 == (decimal) oldMaxPermits)
                    ? 0.0
                    : StoredPermits * MaxPermits / oldMaxPermits;
            }
        }

        protected override long StoredPermitsToWaitTime(double storedPermits, double permitsToTake)
        {
            return 0L;
        }

    }
}
