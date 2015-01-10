﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Tamarind.Base;
using Tamarind.Concurrent;

namespace Tamarind.Tests.Mocks
{
    internal class FakeSleepingStopwatch : SleepingStopwatch
    {
        
        private readonly List<string> events = new List<string>();

        public FakeSleepingStopwatch()
        {
            Instant = 0L;
        }

        public IReadOnlyList<string> Events
        {
            get { return events; }
        }

        public long Instant { get; set; }

        public override long ReadMicroseconds()
        {
            return TimeUnit.Nanoseconds.ToMicroseconds(Instant);
        }

        public override Task SleepMicrosecondsAsync(long micros)
        {
            SleepMicros("R", micros);
            return Task.FromResult(true);
        }

        internal void SleepMillis(int millis)
        {
            SleepMicros("U", TimeUnit.Milliseconds.ToMicroseconds(millis));
        }

        private void SleepMicros(string caption, long amount)
        {
            Instant += TimeUnit.Microseconds.ToNanoseconds(amount);
            events.Add(string.Format("{0}{1:F}", caption, (amount / 1000000.0)));
        }

    }
}
