using System;
using System.Linq;
using System.Threading;

using Tamarind.Annotations;
using Tamarind.Base;

namespace Tamarind.Threading
{
    public sealed class MonitorLock
    {

        private readonly object _locker = new object();

        [PublicAPI]
        public bool IsLockHeld { get { return Monitor.IsEntered(_locker); } }

        [PublicAPI]
        public IDisposable Lock()
        {
            var lockWasTaken = false;
            Monitor.Enter(_locker, ref lockWasTaken);
            return new ActionDisposable(() => Unlock(lockWasTaken));
        }

        private void Unlock(bool lockWasTaken)
        {
            if (lockWasTaken)
            {
                Monitor.Exit(_locker);
            }
        }

    }
}
