using System;
using System.Linq;

using Tamarind.Core;

namespace Tamarind.Concurrent
{
    internal abstract class SmoothRateLimiter : BaseRateLimiter
    {

        /// <summary>
        ///     The time when the next request (no matter its size) will be granted. After granting a
        ///     request, this is pushed further in the future. Large requests push this further than
        ///     small requests.
        /// </summary>
        private long nextFreeTicketMicros = 0L; // Could be either in the past or future

        protected SmoothRateLimiter(SleepingStopwatch stopwatch) : base(stopwatch) {}

        /// <summary>
        ///     Currently stored permits.
        /// </summary>
        protected double StoredPermits { get; set; }

        /// <summary>
        ///     Maximum number of stored permits.
        /// </summary>
        protected double MaxPermits { get; set; }

        /// <summary>
        ///     The interval between two unit requests, at our stable rate. E.g., a stable
        ///     rate of 5 permits per second has a stable interval of 200ms.
        /// </summary>
        protected double StableIntervalMicros { get; private set; }

        protected override sealed double InternalGetRate()
        {
            return CalculateIntervalTicks(StableIntervalMicros);
        }

        protected override sealed void InternalSetRate(double permitsPerSecond, long nowMicros)
        {
            Resync(nowMicros);
            StableIntervalMicros = CalculateIntervalTicks(permitsPerSecond);
            SetRate(permitsPerSecond, StableIntervalMicros);
        }

        protected override sealed long QueryEarliestAvailable(long nowMicros)
        {
            return nextFreeTicketMicros;
        }

        protected override long ReserveEarliestAvailable(int permits, long nowMicros)
        {
            Resync(nowMicros);

            var nextFreeTicketSnapshot = nextFreeTicketMicros;

            var storedPermitsToSpend = System.Math.Min(permits, StoredPermits);
            var freshPermits = permits - storedPermitsToSpend;

            var waitMicros = StoredPermitsToWaitTime(StoredPermits, storedPermitsToSpend)
                             + (long) (freshPermits * StableIntervalMicros);

            nextFreeTicketMicros = nextFreeTicketMicros + waitMicros;
            StoredPermits -= storedPermitsToSpend;

            return nextFreeTicketSnapshot;
        }

        protected abstract void SetRate(double permitsPerSecond, double stableIntervalMicroseconds);

        /// <summary>
        ///     Translates a specified portion of our currently stored permits which we want to
        ///     spend/acquire, into a throttling time. Conceptually, this evaluates the integral
        ///     of the underlying function we use, for the range of
        ///     [(<see cref="storedPermits" /> - <see cref="permitsToTake" />), <see cref="storedPermits" />].
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This always holds <c>0 &lt;= <see cref="permitsToTake" /> &lt;= <see cref="storedPermits" /></c>
        ///     </para>
        /// </remarks>
        protected abstract long StoredPermitsToWaitTime(double storedPermits, double permitsToTake);

        private double CalculateIntervalTicks(double divisor)
        {
            return TimeUnit.Seconds.ToMicroseconds(1L) / divisor;
        }

        private void Resync(long nowMicros)
        {
            if (nowMicros > nextFreeTicketMicros)
            {
                StoredPermits = System.Math.Min(
                    MaxPermits,
                    StoredPermits + (nowMicros - nextFreeTicketMicros) / StableIntervalMicros
                    );
                nextFreeTicketMicros = nowMicros;
            }
        }

    }
}
