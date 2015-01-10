using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Tamarind.Core;

namespace Tamarind.Concurrent
{
    internal abstract class BaseRateLimiter : IRateLimiter
    {

        private readonly object locker = new object();

        private readonly SleepingStopwatch stopwatch;

        internal BaseRateLimiter(SleepingStopwatch stopwatch)
        {
            this.stopwatch = Preconditions.CheckNotNull(stopwatch);
        }

        public double GetRate()
        {
            lock (locker)
            {
                return InternalGetRate();
            }
        }

        public void SetRate(double permitsPerSecond)
        {
            Preconditions.CheckArgument(permitsPerSecond > 0.0 && !double.IsNaN(permitsPerSecond), "rate must be positive");
            lock (locker)
            {
                InternalSetRate(permitsPerSecond, stopwatch.ReadMicroseconds());
            }
        }

        public async Task<TimeSpan> Acquire(int permits = 1)
        {
            var toWait = Reserve(permits);
            await stopwatch.SleepMicrosecondsAsync(toWait);
            return TimeSpan.FromTicks(TimeUnit.Microseconds.ToTicks(toWait));
        }

        public async Task<bool> TryAcquire(int permits = 1, long timeout = 0, TimeUnit unit = null)
        {
            unit = unit ?? TimeUnit.Microseconds;
            var timeoutMicros = System.Math.Max(unit.ToMicroseconds(timeout), 0);
            CheckPermits(permits);

            long toWait;
            lock (locker)
            {
                var now = stopwatch.ReadMicroseconds();
                if (!CanAcquire(now, timeoutMicros))
                {
                    return false;
                }
                toWait = ReserveAndGetWaitLength(permits, now);
            }
            await stopwatch.SleepMicrosecondsAsync(toWait);
            return true;
        }

        internal IRateLimiter FluidSetRate(double permitsPerSecond)
        {
            SetRate(permitsPerSecond);
            return this;
        }

        protected abstract double InternalGetRate();

        protected abstract void InternalSetRate(double permitsPerSecond, long nowMicros);

        protected abstract long QueryEarliestAvailable(long nowMicros);

        protected abstract long ReserveEarliestAvailable(int permits, long nowMicros);

        [DebuggerHidden]
        private bool CanAcquire(long nowMicros, long timeout)
        {
            return QueryEarliestAvailable(nowMicros) - timeout <= nowMicros;
        }

        /// <summary>
        ///     Reserves the given number of permits from this <see cref="RateLimiter" /> for future use, returning
        ///     the timespan until the reservation can be consumed.
        /// </summary>
        /// <returns>time wait until the resource can be acquired, never negative</returns>
        [DebuggerHidden]
        private long Reserve(int permits)
        {
            CheckPermits(permits);
            lock (locker)
            {
                return ReserveAndGetWaitLength(permits, stopwatch.ReadMicroseconds());
            }
        }

        /// <summary>
        ///     Reserves next ticket and returns the wait time that the caller must wait for.
        /// </summary>
        /// <returns>the required wait time, never negative</returns>
        private long ReserveAndGetWaitLength(int permits, long nowMicros)
        {
            var momentAvailable = ReserveEarliestAvailable(permits, nowMicros);
            return System.Math.Max(momentAvailable - nowMicros, 0);
        }

        [DebuggerHidden]
        private static void CheckPermits(int permits)
        {
            Preconditions.CheckArgument(permits > 0, "permits", "Requested permits ({0}) must be positive", permits);
        }

    }

}
