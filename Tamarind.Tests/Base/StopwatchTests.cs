using System;
using System.Linq;

using FluentAssertions;

using Tamarind.Base;

using Xunit;

namespace Tamarind.Tests.Base
{
    public class StopwatchTests
    {

        private readonly FakeTicker _ticker;

        private readonly IStopwatch _stopwatch;

        public StopwatchTests()
        {
            _ticker = new FakeTicker();
            _stopwatch = Stopwatches.Create(_ticker);
        }

        [Fact]
        public void TestCreateStarted()
        {
            Stopwatches.CreateAndStart(_ticker)
                .IsRunning.Should().BeTrue();
        }

        [Fact]
        public void TestCreateUnstarted()
        {
            var sw = Stopwatches.Create();
            sw.IsRunning.Should().BeFalse();
            sw.Elapsed.ShouldBeEquivalentTo(TimeSpan.Zero);
        }

        [Fact]
        public void TestInitialState()
        {
            _stopwatch.IsRunning.Should().BeFalse();
            _stopwatch.Elapsed.Ticks.Should().Be(0);
        }

        [Fact]
        public void TestStart()
        {
            _stopwatch.Start().Should().BeSameAs(_stopwatch);
            _stopwatch.IsRunning.Should().BeTrue();
        }

        [Fact]
        public void TestStartWhileRunning()
        {
            _stopwatch.Start();
            _stopwatch.Start().IsRunning.Should().BeTrue();
        }

        [Fact]
        public void TestStop()
        {
            _stopwatch.Start();
            _stopwatch.Stop().Should().BeSameAs(_stopwatch);
            _stopwatch.IsRunning.Should().BeFalse();
        }

        [Fact]
        public void TestStopOnNew()
        {
            _stopwatch.Stop().Elapsed.ShouldBeEquivalentTo(TimeSpan.Zero);
            _stopwatch.IsRunning.Should().BeFalse();
        }

        [Fact]
        public void TestResetOnNew()
        {
            _ticker.Advance(1);
            _stopwatch.Reset();
            _stopwatch.IsRunning.Should().BeFalse();

            _ticker.Advance(2);
            _stopwatch.Elapsed.ShouldBeEquivalentTo(TimeSpan.Zero);

            _stopwatch.Start();
            _ticker.Advance(3);
            _stopwatch.Elapsed.Ticks.Should().Be(3);
        }

        [Fact]
        public void TestResetWhileRunning()
        {
            _ticker.Advance(1);
            _stopwatch.Start();

            _stopwatch.Elapsed.Ticks.Should().Be(0);

            _ticker.Advance(2);
            _stopwatch.Reset()
                .IsRunning.Should().BeFalse();

            _ticker.Advance(3);
            _stopwatch.Elapsed.Ticks.Should().Be(0);
        }

    }
}
