using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tamarind.Concurrent
{
    public static class Uninterruptibles
    {

        public static void SleepUninterruptibly(TimeSpan sleepFor)
        {
            if (sleepFor == TimeSpan.Zero) { return; }

            var remainingTicks = sleepFor.Ticks;
            var end = DateTime.Now.Ticks + remainingTicks;
            while (true)
            {
                try
                {
                    if (remainingTicks > 0)
                    {
#if PORTABLE || NETFX_CORE
                        using (var mre = new ManualResetEvent(false))
                        {
                            mre.WaitOne(TimeSpan.FromTicks(remainingTicks));
                        }
#else
                        Thread.Sleep(TimeSpan.FromTicks(remainingTicks));
#endif
                    }

                    return;
                }
                catch (Exception)
                {
                    remainingTicks = end - DateTime.Now.Ticks;
                }
            }
        }

        public static async Task SleepUninterruptiblyAsync(TimeSpan sleepFor)
        {
            await Task.Delay(sleepFor).ConfigureAwait(true);
        }

    }
}
