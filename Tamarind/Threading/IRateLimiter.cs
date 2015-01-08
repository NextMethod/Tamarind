using System;
using System.Linq;

namespace Tamarind.Threading
{
    public interface IRateLimiter
    {

        double StableRate { get; set; }

        double Acquire(int permits = 1);

        bool TryAcquire(int permits = 1, TimeSpan? timeout = null);
    }
}
