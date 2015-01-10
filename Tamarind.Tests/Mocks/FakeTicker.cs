using System;
using System.Linq;
using System.Threading;

using Tamarind.Core;

namespace Tamarind.Tests.Mocks
{
    public class FakeTicker : Ticker
    {

        private long autoIncrementStep;

        private long ticks;

        public FakeTicker Advance(TimeSpan step)
        {
            return Advance(step.Ticks);
        }

        public FakeTicker Advance(long tcks)
        {
            Preconditions.CheckArgument(tcks >= 0);
            Interlocked.Add(ref ticks, tcks);
            return this;
        }

        public FakeTicker SetAutoIncrementStep(TimeSpan step)
        {
            autoIncrementStep = step.Ticks;
            return this;
        }

        public override long Read()
        {
            return Interlocked.Add(ref ticks, autoIncrementStep);
        }

    }
}
