using System;
using System.Diagnostics;
using System.Linq;

namespace Tamarind.Base
{
    internal sealed class SystemStopwatch : IStopwatch
    {

        private readonly Stopwatch _stopwatch;

        public SystemStopwatch()
        {
            _stopwatch = new Stopwatch();
        }

        public bool IsRunning
        {
            get { return _stopwatch.IsRunning; }
        }

        public TimeSpan Elapsed
        {
            get { return _stopwatch.Elapsed; }
        }

        public IStopwatch Reset()
        {
            _stopwatch.Reset();
            return this;
        }

        public IStopwatch Start()
        {
            _stopwatch.Start();
            return this;
        }

        public IStopwatch Stop()
        {
            _stopwatch.Stop();
            return this;
        }

        public override string ToString()
        {
            return _stopwatch.ToString();
        }

    }
}
