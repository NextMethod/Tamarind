using System;
using System.Diagnostics;
using System.Linq;

namespace Tamarind.Base
{
    internal class SystemStopwatchBackedTicker : Ticker
    {

        private readonly Stopwatch _sw;


        public SystemStopwatchBackedTicker()
        {
            _sw = new Stopwatch();
            _sw.Start();
        }

        public override long Read()
        {
            return _sw.ElapsedTicks;
        }

    }
}
