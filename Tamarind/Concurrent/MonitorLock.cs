using System;
using System.Linq;
using System.Threading;

using Tamarind.Annotations;
using Tamarind.Core;

namespace Tamarind.Concurrent
{
    public sealed class MonitorLock
    {

        private readonly object locker = new object();

        [PublicAPI]
        public bool IsLockHeld { get { return Monitor.IsEntered(locker); } }

        [PublicAPI]
        public IDisposable Lock()
        {
            var lockWasTaken = false;
            Monitor.Enter(locker, ref lockWasTaken);
            return new ActionDisposable(() => Unlock(lockWasTaken));
        }

        private void Unlock(bool lockWasTaken)
        {
            if (lockWasTaken)
            {
                Monitor.Exit(locker);
            }
        }

    }
}
