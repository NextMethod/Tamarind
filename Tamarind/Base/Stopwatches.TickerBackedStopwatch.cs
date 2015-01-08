using System;
using System.Linq;

namespace Tamarind.Base
{
    internal sealed class TickerBackedStopwatch : IStopwatch
    {

        private long _elapsedNanos;

        private long _startTick;

        private readonly Ticker _ticker;

        public TickerBackedStopwatch(Ticker ticker)
        {
            _ticker = ticker;
            IsRunning = false;
        }

        public TimeSpan Elapsed
        {
            get
            {
                var ticks = IsRunning ? _ticker.Read() - _startTick + _elapsedNanos : _elapsedNanos;
                return new TimeSpan(ticks);
            }
        }

        public bool IsRunning { get; private set; }

        public IStopwatch Reset()
        {
            _elapsedNanos = 0;
            IsRunning = false;
            return this;
        }

        public IStopwatch Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                _startTick = _ticker.Read();
            }
            return this;
        }

        public IStopwatch Stop()
        {
            var tick = _ticker.Read();
            var wasRunning = IsRunning;
            IsRunning = false;

            if (wasRunning) _elapsedNanos += tick - _startTick;

            return this;
        }

        public override string ToString()
        {
            return Elapsed.ToString();
        }

    }
}
