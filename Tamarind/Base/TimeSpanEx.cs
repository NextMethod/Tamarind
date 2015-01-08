using System;
using System.Linq;

namespace Tamarind.Base
{
    public static class TimeSpanEx
    {

        private const long TicksPerMicrosecond = 10;

        public static TimeSpan FromMicroseconds(double value)
        {
            Preconditions.CheckArgument(Double.IsNaN(value) == false);
            return new TimeSpan((long) value * TicksPerMicrosecond);
        }

    }
}
