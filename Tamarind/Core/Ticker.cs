using System;
using System.Linq;

namespace Tamarind.Core
{
    /// <summary>
    ///     A time source; returns a time value representing the number of ticks elapsed since some fixed but arbitrary point
    ///     in time.
    ///     <para><strong>Warning:</strong> this class can only be used to measure elapsed time, not wall time.</para>
    /// </summary>
    public abstract class Ticker
    {

        private static readonly Ticker SysTicker = new SystemTicker();

        /// <summary>
        ///     Returns the number of ticks elapsed since this ticker's fixed point of reference.
        /// </summary>
        public abstract long Read();

        /// <summary>
        ///     A ticker that reads the current time using <see cref="DateTime.Now" />.
        /// </summary>
        public static Ticker SystemTicker()
        {
            return SysTicker;
        }

        /// <summary>
        ///     A ticker backed by <see cref="System.Diagnostics.Stopwatch" />.
        /// </summary>
        public static Ticker SystemStopwatchBackedTicker()
        {
            return new SystemStopwatchBackedTicker();
        }

    }

}
