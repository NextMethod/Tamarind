using System;
using System.Linq;

namespace Tamarind.Base
{
    internal class SystemTicker : Ticker
    {

        public override long Read()
        {
            return DateTime.Now.Ticks;
        }

    }
}
