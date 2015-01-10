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

            var interrupted = false;
            try
            {
                var remainingTicks = sleepFor.Ticks;
                var end = DateTime.Now.Ticks + remainingTicks;
                while (true)
                {
                    try
                    {
                        Thread.Sleep(TimeSpan.FromTicks(remainingTicks));
                        return;
                    }
                    catch (ThreadInterruptedException)
                    {
                        interrupted = true;
                        remainingTicks = end - DateTime.Now.Ticks;
                    }
                }
            }
            finally
            {
                if (interrupted)
                {
                    Thread.CurrentThread.Interrupt();
                }
            }
        }

        public static async Task SleepUninterruptiblyAsync(TimeSpan sleepFor)
        {
            await Task.Delay(sleepFor);
        }

    }
}
