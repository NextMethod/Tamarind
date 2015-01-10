using System;
using System.Diagnostics;
using System.Linq;

namespace Tamarind.Base
{
    internal class SystemStopwatchBackedTicker : Ticker
    {

        private readonly Stopwatch sw;

        public SystemStopwatchBackedTicker()
        {
            sw = new Stopwatch();
            sw.Start();
        }

        public override long Read()
        {
            return sw.ElapsedTicks;
        }

    }
}
