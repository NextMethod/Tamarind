using System;
using System.Linq;
using System.Threading.Tasks;

using Tamarind.Core;

namespace Tamarind.Concurrent
{
    /// <summary>
    ///     A rate limiter. Conceptually, a rate limiter distributes permits at a
    ///     configurable rate. Each <see cref="Acquire" /> blocks if necessary until a permit is
    ///     available, and then takes it. Once acquired, permits need not be released.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Rate limiters are often used to restrict the rate at which some
    ///         physical or logical resource is accessed. This is in contrast to
    ///         <see cref="System.Threading.Semaphore" /> which restricts the number of concurrent
    ///         accesses instead of the rate (note though that concurrency and rate are closely related,
    ///         e. g. see <a href="http://en.wikipedia.org/wiki/Little's_law">Little's Law</a>).
    ///     </para>
    ///     <para>
    ///         An <see cref="IRateLimiter" /> is defined primarily by the rate at which permits
    ///         are issued. Absent additional configuration, permits will be distributed at a
    ///         fixed rate, defined in terms of permits per second. Permits will be distributed
    ///         smoothly, with the delay between individual permits being adjusted to ensure
    ///         that the configured rate is maintained.
    ///     </para>
    ///     <para>
    ///         It is possible to configure an <see cref="IRateLimiter" /> to have a warmup
    ///         period during which time the permits issued each second steadily increases until
    ///         it hits the stable rate.
    ///     </para>
    ///     <para>
    ///         It is important to note that the number of permits requested <em>never</em>
    ///         affect the throttling of the request itself (an invocation to <c>Acquire(1)</c>
    ///         and an invocation to <c>Acquire(1000)</c> will result in exactly the same throttling, if any),
    ///         but it affects the throttling of the <em>next</em> request. I.e. if an expensive task
    ///         arrives at an idle <c>IRateLimiter</c>, it will be granted immediately, but it is the <em>next</em>
    ///         request that will experience extra throttling, thus paying for the cost of the expensive task.
    ///     </para>
    ///     <note type="note"><c>IRateLimiter</c> does not provide fairness guarantees.</note>
    ///     <para>
    ///         See
    ///         <a href="http://docs.guava-libraries.googlecode.com/git-history/release/javadoc/com/google/common/util/concurrent/RateLimiter.html">
    ///             Guava
    ///             Reference
    ///         </a>
    ///     </para>
    /// </remarks>
    public interface IRateLimiter
    {

        /// <summary>
        ///     Returns the stable rate (as <c>permits per second</c>) with which this
        ///     <see cref="IRateLimiter" /> is configured with. The initial value of this is
        ///     the same as the <c>permitsPerSecond</c> argument passed to the factory method
        ///     that produced this <see cref="IRateLimiter" />, and it is only updated after
        ///     invocations to <see cref="SetRate(double)" />.
        /// </summary>
        double GetRate();

        /// <summary>
        ///     Updates the stable rate of this <see cref="IRateLimiter" />, that is, the
        ///     <paramref name="permitsPerSecond" /> argument provided in the factory method that
        ///     constructed the <c>IRateLimiter</c>. Currently throttled threads will <strong>not</strong>
        ///     be awakened as a result of this invocation, thus they do not observe the new rate;
        ///     only subsequent requests will.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Note though that, since each request repays (by waiting, if necessary) the cost of the
        ///         <em>previous</em> request, this means that the very next request after an invocation
        ///         to <see cref="SetRate(double)" /> will not be affected by the new rate;
        ///         it will pay the cost of the previous request, which is in terms of the previous rate.
        ///     </para>
        ///     <para>
        ///         The behavior of the <see cref="IRateLimiter" /> is not modified in any other way,
        ///         e.g. if the <see cref="IRateLimiter" /> was configured with a warmup period of 20 seconds,
        ///         it still has a warmup period of 20 seconds after this method invocation.
        ///     </para>
        /// </remarks>
        /// <param name="permitsPerSecond">the new stable rate of this <see cref="IRateLimiter" /></param>
        /// <exception cref="ArgumentException">if <paramref name="permitsPerSecond" /> is negative or zero</exception>
        void SetRate(double permitsPerSecond);

        /// <summary>
        ///     Acquires a given number of permits from this <see cref="IRateLimiter" />, blocking
        ///     until the request can be granted. Tells the amount of time slept, if any.
        /// </summary>
        /// <param name="permits">the number of permits to acquire with a default of 1</param>
        /// <returns>time spent sleeping to enforce rate; <see cref="System.TimeSpan.Zero" /> is not rate-limited</returns>
        Task<TimeSpan> Acquire(int permits = 1);

        /// <summary>
        ///     Aquires the given number of permits from this <see cref="IRateLimiter" /> if it can be
        ///     obtained without exceeding the specified <see cref="timeout" />, or returns <c>false</c>
        ///     immediately (without waiting) if the permits would not have been granted before
        ///     the timeout expired
        /// </summary>
        /// <param name="permits">the number of permits to acquire with a default of 1</param>
        /// <param name="timeout">
        ///     the maximum time to wait for the permits with a default of 0.
        ///     Negative values are treated as zero.
        /// </param>
        /// <param name="unit">the time unit of the timeout argument. Defaults to <see cref="TimeUnit.Microseconds" /></param>
        /// <returns><c>true</c> if the permits were acquired, <c>false</c> otherwise</returns>
        Task<bool> TryAcquire(int permits = 1, long timeout = 0, TimeUnit unit = null);

    }
}
