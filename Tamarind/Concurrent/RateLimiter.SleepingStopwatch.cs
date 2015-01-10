using System;
using System.Linq;
using System.Threading.Tasks;

using Tamarind.Core;

namespace Tamarind.Concurrent
{
    internal abstract class SleepingStopwatch
    {

        public abstract long ReadMicroseconds();

        public abstract Task SleepMicrosecondsAsync(long micros);

        public static SleepingStopwatch CreateDefault()
        {
            return new DefaultSleepingStopwatch();
        }

        private sealed class DefaultSleepingStopwatch : SleepingStopwatch
        {

            private readonly IStopwatch stopwatch = Stopwatches.CreateAndStart();

            public override long ReadMicroseconds()
            {
                return TimeUnit.Ticks.ToMicroseconds(stopwatch.Elapsed.Ticks);
            }

            public override async Task SleepMicrosecondsAsync(long micros)
            {
                if (micros > 0)
                {
                    await Uninterruptibles.SleepUninterruptiblyAsync(TimeSpan.FromTicks(
                        TimeUnit.Microseconds.ToTicks(micros)
                        ));
                }
            }

        }

    }
}