using System;
using System.Linq;
using System.Threading;

using Tamarind.Base;

namespace Tamarind.Tests.Base
{
    public class FakeTicker : Ticker
    {

        private long _ticks = 0;

        private long _autoIncrementStep;

        public FakeTicker Advance(TimeSpan step)
        {
            return Advance(step.Ticks);
        }

        public FakeTicker Advance(long ticks)
        {
            Preconditions.CheckArgument(ticks >= 0);
            Interlocked.Add(ref _ticks, ticks);
            return this;
        }

        public FakeTicker SetAutoIncrementStep(TimeSpan step)
        {
            _autoIncrementStep = step.Ticks;
            return this;
        }

        public override long Read()
        {
            return Interlocked.Add(ref _ticks, _autoIncrementStep);
        }

    }
}
