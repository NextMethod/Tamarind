using System;
using System.Linq;
using System.Threading;

using Tamarind.Base;

namespace Tamarind.Threading
{
    public sealed class SimpleLock
    {

        internal SimpleLock()
        { }

        public IDisposable Lock(object locker)
        {
            Thread.BeginCriticalRegion();
            var lockWasTaken = false;
            Monitor.Enter(locker, ref lockWasTaken);
            return new ActionDisposable(() => OnUnlock(locker, lockWasTaken));
        }

        private void OnUnlock(object locker, bool lockWasTaken)
        {
            if (lockWasTaken)
            {
                Monitor.Exit(locker);
            }
        }

    }
}
