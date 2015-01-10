using System;
using System.Linq;

namespace Tamarind.Core
{
    internal class SystemTicker : Ticker
    {

        public override long Read()
        {
            return DateTime.Now.Ticks;
        }

    }
}
