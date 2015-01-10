using System;
using System.Linq;

using Tamarind.Annotations;

namespace Tamarind.Core
{
    internal sealed class TickerBackedStopwatch : IStopwatch
    {

        private long elapsedNanos;

        private long startTick;

        public TickerBackedStopwatch(Ticker ticker)
        {
            Ticker = ticker;
            IsRunning = false;
        }

        [VisibleForTesting]
        internal Ticker Ticker { get; private set; }

        public TimeSpan Elapsed
        {
            get
            {
                var ticks = IsRunning ? Ticker.Read() - startTick + elapsedNanos : elapsedNanos;
                return new TimeSpan(ticks);
            }
        }

        public bool IsRunning { get; private set; }

        public IStopwatch Reset()
        {
            elapsedNanos = 0;
            IsRunning = false;
            return this;
        }

        public IStopwatch Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                startTick = Ticker.Read();
            }
            return this;
        }

        public IStopwatch Stop()
        {
            var tick = Ticker.Read();
            var wasRunning = IsRunning;
            IsRunning = false;

            if (wasRunning) { elapsedNanos += tick - startTick; }

            return this;
        }

        public override string ToString()
        {
            return Elapsed.ToString();
        }

    }
}
