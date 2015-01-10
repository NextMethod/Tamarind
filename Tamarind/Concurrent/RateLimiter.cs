using System;
using System.Diagnostics;
using System.Linq;

using Tamarind.Annotations;
using Tamarind.Core;

namespace Tamarind.Concurrent
{
    public static class RateLimiter
    {

        /// <summary>
        ///     Creates a <see cref="IRateLimiter" /> with the specified stable throughput, given as
        ///     "permits per second" (commonly referred to as <em>QPS</em>, queries per second).
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The returned <see cref="IRateLimiter" /> ensures that on average no more than
        ///         <paramref name="permitsPerSecond" /> are issued during any given second, with sustained
        ///         requests being smoothly spread over each second. When the incoming request rate exceeds
        ///         <paramref name="permitsPerSecond" /> the rate limiter will release one permit every
        ///         <c>(1.0 / <paramref name="permitsPerSecond" />)</c> seconds. When the rate limiter is unsed,
        ///         bursts of up to <paramref name="permitsPerSecond" /> permits will be allowed, with subsequent
        ///         requests being smoothly limited at the stable rate of <paramref name="permitsPerSecond" />.
        ///     </para>
        /// </remarks>
        /// <param name="permitsPerSecond">
        ///     the rate of the returned <see cref="IRateLimiter" />, measured in
        ///     how many permits become available per second
        /// </param>
        /// <exception cref="ArgumentException">if <paramref name="permitsPerSecond" /> is negative or zero</exception>
        public static IRateLimiter Create(double permitsPerSecond)
        {
            return Create(permitsPerSecond, SleepingStopwatch.CreateDefault());
        }

        /// <summary>
        ///     Creates a <see cref="IRateLimiter" /> with the specified stable throughput, given as
        ///     "permits per second" (commonly referred to as <em>QPS</em>, queries per second), and a
        ///     <em>warmup period</em>, during which the <see cref="IRateLimiter" /> smoothly ramps up its rate,
        ///     until it reaches its maximum rate at the end of the period (as long as there are enough requests to
        ///     saturate it). Similarly, if the <see cref="IRateLimiter" /> is left <em>unsed</em> for a duration of
        ///     <paramref name="warmupPeriod" />, it will gradually return to its "cold" state,
        ///     i.e. it will go through the same warming up process as when it was first created.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The returned <see cref="IRateLimiter" /> is intended for cases where the resource that actually
        ///         fulfills the requests (e.g., a remote server) needs "warmup" time, rather than
        ///         being immediately accessed at the stable (maximum) rate.
        ///     </para>
        ///     <para>
        ///         The returned <see cref="IRateLimiter" /> starts in a "cold" state (i.e. the warmup period
        ///         will follow), and if it is left unused for long enough, it will return to that state.
        ///     </para>
        /// </remarks>
        /// <param name="permitsPerSecond">
        ///     the rate of the returned <see cref="IRateLimiter" />, measured in
        ///     how many permits become available per second
        /// </param>
        /// <param name="warmupPeriod">
        ///     the duration of the period where the <see cref="IRateLimiter" /> ramps
        ///     up its rate, before reaching its stable (maximum) rate
        /// </param>
        /// <exception cref="ArgumentException">
        ///     if <paramref name="permitsPerSecond" /> is negative or zero or
        ///     <paramref name="warmupPeriod" /> is negative
        /// </exception>
        public static IRateLimiter Create(double permitsPerSecond, TimeSpan warmupPeriod)
        {
            Preconditions.CheckArgument(warmupPeriod >= TimeSpan.Zero, "warmupPeriod", "must not be negative %s", warmupPeriod);

            return Create(permitsPerSecond, warmupPeriod, SleepingStopwatch.CreateDefault());
        }

        [VisibleForTesting, DebuggerHidden]
        internal static IRateLimiter Create(double permitsPerSecond, SleepingStopwatch stopwatch)
        {
            return new SmoothBurstyRateLimiter(stopwatch, 1.0)
                .FluidSetRate(permitsPerSecond);
        }

        [VisibleForTesting, DebuggerHidden]
        internal static IRateLimiter Create(double permitsPerSecond, TimeSpan warmupPeriod, SleepingStopwatch stopwatch)
        {
            return new SmoothWarmingUpRateLimiter(stopwatch, warmupPeriod)
                .FluidSetRate(permitsPerSecond);
        }

    }
}
