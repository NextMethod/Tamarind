using System;
using System.Linq;

namespace Tamarind.Base
{
    public static class TimeSpanEx
    {

        public static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        internal const long TicksPerMicrosecond = 10;

        public static TimeSpan Max(this TimeSpan This, TimeSpan other)
        {
            return This > other ? This : other;
        }

        public static TimeSpan Min(this TimeSpan This, TimeSpan other)
        {
            return This < other ? This : other;
        }

        public static long TotalMicroseconds(this TimeSpan This)
        {
            return This.Ticks / TicksPerMicrosecond;
        }

    }
}
