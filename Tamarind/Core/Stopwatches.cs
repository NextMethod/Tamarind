using System;
using System.Linq;

using Tamarind.Annotations;

namespace Tamarind.Core
{
    public static class Stopwatches
    {

        /// <summary>
        ///     Creates (but does not start) a new <see cref="IStopwatch" />.
        /// </summary>
        [PublicAPI]
        public static IStopwatch Create()
        {
            return new TickerBackedStopwatch(Ticker.SystemStopwatchBackedTicker());
        }

        /// <summary>
        ///     Creates (but does not start) as new <see cref="IStopwatch" /> using the specified time source.
        /// </summary>
        [PublicAPI]
        public static IStopwatch Create(Ticker ticker)
        {
            if (ticker == null) { return Create(); }

            return new TickerBackedStopwatch(ticker);
        }

        /// <summary>
        ///     Creates (and starts) as new <see cref="IStopwatch" />.
        /// </summary>
        [PublicAPI]
        public static IStopwatch CreateAndStart()
        {
            return Create().Start();
        }

        /// <summary>
        ///     Creates (and starts) as new <see cref="IStopwatch" /> using the specified time source.
        /// </summary>
        [PublicAPI]
        public static IStopwatch CreateAndStart(Ticker ticker)
        {
            return Create(ticker).Start();
        }

    }

}
