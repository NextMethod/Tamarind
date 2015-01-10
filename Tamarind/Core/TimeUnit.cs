using System;
using System.Linq;

using Tamarind.Annotations;

namespace Tamarind.Core
{
    /// <summary>
    ///     C# port of Doug Lea's Java public domain version.
    /// </summary>
    public sealed class TimeUnit
    {
        #region Handy utilities for conversion methods

        internal const long NanosInNanosecond = 1L;

        internal const long NanosInTick = NanosInNanosecond * 100L;

        internal const long NanosInMicrosecond = NanosInNanosecond * 1000L;

        internal const long NanosInMillisecond = NanosInMicrosecond * 1000L;

        internal const long NanosInSecond = NanosInMillisecond * 1000L;

        internal const long NanosInMinute = NanosInSecond * 60L;

        internal const long NanosInHour = NanosInMinute * 60L;

        internal const long NanosInDay = NanosInHour * 24L;

        internal const long Max = long.MaxValue;


        /// <summary>
        ///     Scale <paramref name="d" /> by <paramref name="m" />, checking for overflow.
        /// </summary>
        private static long X(long d, long m, long over)
        {
            if (d > over) { return long.MaxValue; }
            if (d < -over) { return long.MinValue; }
            return d * m;
        }

        #endregion

        #region "Conversion" Methods

        [PublicAPI]
        public Func<long, long> ToNanoseconds { get; private set; }

        [PublicAPI]
        public Func<long, long> ToMicroseconds { get; private set; }

        [PublicAPI]
        public Func<long, long> ToMilliseconds { get; private set; }

        [PublicAPI]
        public Func<long, long> ToSeconds { get; private set; }

        [PublicAPI]
        public Func<long, long> ToMinutes { get; private set; }

        [PublicAPI]
        public Func<long, long> ToHours { get; private set; }

        [PublicAPI]
        public Func<long, long> ToDays { get; private set; }

        [PublicAPI]
        public Func<long, long> ToTicks { get; private set; }

        [PublicAPI]
        public Func<long, TimeUnit, long> Convert { get; private set; }

        private Func<long, long, int> ExcessNanoseconds { get; set; }

        #endregion

        [PublicAPI]
        public static readonly TimeUnit Nanoseconds = new TimeUnit
        {
            ToNanoseconds = d => d,
            ToMicroseconds = d => d / (NanosInMicrosecond / NanosInNanosecond),
            ToMilliseconds = d => d / (NanosInMillisecond / NanosInNanosecond),
            ToSeconds = d => d / (NanosInSecond / NanosInNanosecond),
            ToMinutes = d => d / (NanosInMinute / NanosInNanosecond),
            ToHours = d => d / (NanosInHour / NanosInNanosecond),
            ToDays = d => d / (NanosInDay / NanosInNanosecond),
            ToTicks = d => d / (NanosInTick / NanosInNanosecond),
            Convert = (d, u) => u.ToNanoseconds(d),
            ExcessNanoseconds = (d, m) => (int) (d - (m * NanosInMillisecond))
        };

        [PublicAPI]
        public static readonly TimeUnit Ticks = new TimeUnit
        {
            ToNanoseconds = d => X(d, NanosInTick / NanosInNanosecond, Max / (NanosInTick / NanosInNanosecond)),
            ToMicroseconds = d => d / (NanosInMicrosecond / NanosInTick),
            ToMilliseconds = d => d / (NanosInMillisecond / NanosInTick),
            ToSeconds = d => d / (NanosInSecond / NanosInTick),
            ToMinutes = d => d / (NanosInMinute / NanosInTick),
            ToHours = d => d / (NanosInHour / NanosInTick),
            ToDays = d => d / (NanosInDay / NanosInTick),
            ToTicks = d => d,
            Convert = (d, u) => u.ToMicroseconds(d),
            ExcessNanoseconds = (d, m) => (int) ((d * NanosInTick) - (m * NanosInMillisecond))
        };

        [PublicAPI]
        public static readonly TimeUnit Microseconds = new TimeUnit
        {
            ToNanoseconds = d => X(d, NanosInMicrosecond / NanosInNanosecond, Max / (NanosInMicrosecond / NanosInNanosecond)),
            ToMicroseconds = d => d,
            ToMilliseconds = d => d / (NanosInMillisecond / NanosInMicrosecond),
            ToSeconds = d => d / (NanosInSecond / NanosInMicrosecond),
            ToMinutes = d => d / (NanosInMinute / NanosInMicrosecond),
            ToHours = d => d / (NanosInHour / NanosInMicrosecond),
            ToDays = d => d / (NanosInDay / NanosInMicrosecond),
            ToTicks = d => X(d, NanosInMicrosecond / NanosInTick, Max / (NanosInMicrosecond / NanosInTick)),
            Convert = (d, u) => u.ToMicroseconds(d),
            ExcessNanoseconds = (d, m) => (int) ((d * NanosInMicrosecond) - (m * NanosInMillisecond))
        };

        [PublicAPI]
        public static readonly TimeUnit Milliseconds = new TimeUnit
        {
            ToNanoseconds = d => X(d, NanosInMillisecond / NanosInNanosecond, Max / (NanosInMillisecond / NanosInNanosecond)),
            ToMicroseconds = d => X(d, NanosInMillisecond / NanosInMicrosecond, Max / (NanosInMillisecond / NanosInMicrosecond)),
            ToMilliseconds = d => d,
            ToSeconds = d => d / (NanosInSecond / NanosInMillisecond),
            ToMinutes = d => d / (NanosInMinute / NanosInMillisecond),
            ToHours = d => d / (NanosInHour / NanosInMillisecond),
            ToDays = d => d / (NanosInDay / NanosInMillisecond),
            ToTicks = d => X(d, NanosInMillisecond / NanosInTick, Max / (NanosInMillisecond / NanosInTick)),
            Convert = (d, u) => u.ToMilliseconds(d),
            ExcessNanoseconds = (d, m) => 0
        };

        [PublicAPI]
        public static readonly TimeUnit Seconds = new TimeUnit
        {
            ToNanoseconds = d => X(d, NanosInSecond / NanosInNanosecond, Max / (NanosInSecond / NanosInNanosecond)),
            ToMicroseconds = d => X(d, NanosInSecond / NanosInMicrosecond, Max / (NanosInSecond / NanosInMicrosecond)),
            ToMilliseconds = d => X(d, NanosInSecond / NanosInMillisecond, Max / (NanosInSecond / NanosInMillisecond)),
            ToSeconds = d => d,
            ToMinutes = d => d / (NanosInMinute / NanosInSecond),
            ToHours = d => d / (NanosInHour / NanosInSecond),
            ToDays = d => d / (NanosInDay / NanosInSecond),
            ToTicks = d => X(d, NanosInSecond / NanosInTick, Max / (NanosInSecond / NanosInTick)),
            Convert = (d, u) => u.ToSeconds(d),
            ExcessNanoseconds = (d, m) => 0
        };

        [PublicAPI]
        public static readonly TimeUnit Minutes = new TimeUnit
        {
            ToNanoseconds = d => X(d, NanosInMinute / NanosInNanosecond, Max / (NanosInMinute / NanosInNanosecond)),
            ToMicroseconds = d => X(d, NanosInMinute / NanosInMicrosecond, Max / (NanosInMinute / NanosInMicrosecond)),
            ToMilliseconds = d => X(d, NanosInMinute / NanosInMillisecond, Max / (NanosInMinute / NanosInMillisecond)),
            ToSeconds = d => X(d, NanosInMinute / NanosInSecond, Max / (NanosInMinute / NanosInSecond)),
            ToMinutes = d => d,
            ToHours = d => d / (NanosInHour / NanosInMinute),
            ToDays = d => d / (NanosInDay / NanosInMinute),
            ToTicks = d => X(d, NanosInMinute / NanosInTick, Max / (NanosInMinute / NanosInTick)),
            Convert = (d, u) => u.ToMinutes(d),
            ExcessNanoseconds = (d, m) => 0
        };

        [PublicAPI]
        public static readonly TimeUnit Hours = new TimeUnit
        {
            ToNanoseconds = d => X(d, NanosInHour / NanosInNanosecond, Max / (NanosInHour / NanosInNanosecond)),
            ToMicroseconds = d => X(d, NanosInHour / NanosInMicrosecond, Max / (NanosInHour / NanosInMicrosecond)),
            ToMilliseconds = d => X(d, NanosInHour / NanosInMillisecond, Max / (NanosInHour / NanosInMillisecond)),
            ToSeconds = d => X(d, NanosInHour / NanosInSecond, Max / (NanosInHour / NanosInSecond)),
            ToMinutes = d => X(d, NanosInHour / NanosInMinute, Max / (NanosInHour / NanosInMinute)),
            ToHours = d => d,
            ToDays = d => d / (NanosInDay / NanosInHour),
            ToTicks = d => X(d, NanosInHour / NanosInTick, Max / (NanosInHour / NanosInTick)),
            Convert = (d, u) => u.ToHours(d),
            ExcessNanoseconds = (d, m) => 0
        };

        [PublicAPI]
        public static readonly TimeUnit Days = new TimeUnit
        {
            ToNanoseconds = d => X(d, NanosInDay / NanosInNanosecond, Max / (NanosInDay / NanosInNanosecond)),
            ToMicroseconds = d => X(d, NanosInDay / NanosInMicrosecond, Max / (NanosInDay / NanosInMicrosecond)),
            ToMilliseconds = d => X(d, NanosInDay / NanosInMillisecond, Max / (NanosInDay / NanosInMillisecond)),
            ToSeconds = d => X(d, NanosInDay / NanosInSecond, Max / (NanosInDay / NanosInSecond)),
            ToMinutes = d => X(d, NanosInDay / NanosInMinute, Max / (NanosInDay / NanosInMinute)),
            ToHours = d => X(d, NanosInDay / NanosInHour, Max / (NanosInDay / NanosInHour)),
            ToTicks = d => X(d, NanosInHour / NanosInTick, Max / (NanosInHour / NanosInTick)),
            ToDays = d => d,
            Convert = (d, u) => u.ToDays(d),
            ExcessNanoseconds = (d, m) => 0
        };

    }
}
