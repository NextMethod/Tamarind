using System;
using System.Linq;

using Tamarind.Core;

namespace Tamarind.Concurrent
{
    internal sealed class SmoothWarmingUpRateLimiter : SmoothRateLimiter
    {

        private double halfPermits;

        /// <summary>
        ///     The slop of the line from the stable interval (when permits == 0), to the cold interval (when permits ==
        ///     <see cref="SmoothRateLimiter.MaxPermits" />)
        /// </summary>
        private double slope;

        private readonly long warmupPeriodMicroseconds;

        public SmoothWarmingUpRateLimiter(SleepingStopwatch stopwatch, TimeSpan warmupPeriod) : base(stopwatch)
        {
            warmupPeriodMicroseconds = TimeUnit.Ticks.ToMicroseconds(warmupPeriod.Ticks);
        }

        protected override void SetRate(double permitsPerSecond, double stableIntervalMicroseconds)
        {
            var oldMaxPermits = MaxPermits;
            MaxPermits = warmupPeriodMicroseconds / stableIntervalMicroseconds;
            halfPermits = MaxPermits / 2.0;

            // Stable interval is x, cold is 3x, so on averate it's 2x. Double the time -> halve the rate
            var coldIntervalMicroseconds = stableIntervalMicroseconds * 3.0;
            slope = (coldIntervalMicroseconds - stableIntervalMicroseconds) / halfPermits;

            if (double.IsPositiveInfinity(oldMaxPermits))
            {
                StoredPermits = 0.0;
            }
            else
            {
                StoredPermits = ((decimal) 0.0 == (decimal) oldMaxPermits)
                    ? MaxPermits // initial state is cold
                    : StoredPermits * MaxPermits / oldMaxPermits;
            }
        }

        protected override long StoredPermitsToWaitTime(double storedPermits, double permitsToTake)
        {
            var availablePermitsAboveHalf = storedPermits - halfPermits;

            long micros = 0;
            // measuring the integral on the right part of the function (the climbing line)
            if (availablePermitsAboveHalf > 0.0)
            {
                var permitsAboveHalfToTake = System.Math.Min(availablePermitsAboveHalf, permitsToTake);
                micros = (long) (permitsAboveHalfToTake * (PermitsToTime(availablePermitsAboveHalf) + PermitsToTime(availablePermitsAboveHalf - permitsAboveHalfToTake)) / 2.0);
                permitsToTake -= permitsAboveHalfToTake;
            }
            // measuring the integral on the left part of the function (the horizontal line)
            micros += (long) (StableIntervalMicros * permitsToTake);
            return micros;
        }

        private double PermitsToTime(double permits)
        {
            return StableIntervalMicros + permits * slope;
        }

    }
}
