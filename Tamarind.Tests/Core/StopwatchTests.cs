using System;
using System.Linq;

using FluentAssertions;

using Tamarind.Core;
using Tamarind.Tests.Mocks;

using Xunit;

namespace Tamarind.Tests.Core
{
    public class StopwatchTests
    {

        private readonly IStopwatch stopwatch;

        private readonly FakeTicker ticker;

        public StopwatchTests()
        {
            ticker = new FakeTicker();
            stopwatch = Stopwatches.Create(ticker);
        }

        [Fact]
        public void TestEmptyCreateShouldUseSystemStopwatch()
        {
            var sw = Stopwatches.Create();
            sw.Should().BeOfType<TickerBackedStopwatch>()
                .Which.Ticker
                .Should().BeOfType<SystemStopwatchBackedTicker>();
        }

        [Fact]
        public void TestEmptyCreateStartedShouldUseSystemStopwatch()
        {
            var sw = Stopwatches.CreateAndStart();
            sw.Should().BeOfType<TickerBackedStopwatch>()
                .Which.Ticker
                .Should().BeOfType<SystemStopwatchBackedTicker>();
        }

        [Fact]
        public void TestCreateStarted()
        {
            Stopwatches.CreateAndStart(ticker)
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
            IsRunning.Should().BeFalse();
            Elapsed.Ticks.Should().Be(0);
        }

        [Fact]
        public void TestStart()
        {
            Start().Should().BeSameAs(stopwatch);
            IsRunning.Should().BeTrue();
        }

        [Fact]
        public void TestStartWhileRunning()
        {
            Start();
            Start().IsRunning.Should().BeTrue();
        }

        [Fact]
        public void TestStop()
        {
            Start();
            Stop().Should().BeSameAs(stopwatch);
            IsRunning.Should().BeFalse();
        }

        [Fact]
        public void TestStopOnNew()
        {
            Stop().Elapsed.ShouldBeEquivalentTo(TimeSpan.Zero);
            IsRunning.Should().BeFalse();
        }

        [Fact]
        public void TestResetOnNew()
        {
            Advance(1);
            Reset();
            IsRunning.Should().BeFalse();

            Advance(2);
            Elapsed.ShouldBeEquivalentTo(TimeSpan.Zero);

            Start();
            Advance(3);
            Elapsed.Ticks.Should().Be(3);
        }

        [Fact]
        public void TestResetWhileRunning()
        {
            Advance(1);
            Start();

            Elapsed.Ticks.Should().Be(0);

            Advance(2);
            Reset()
                .IsRunning.Should().BeFalse();

            Advance(3);
            Elapsed.Ticks.Should().Be(0);
        }

        [Fact]
        public void TestElapsedWhileRunning()
        {
            Advance(78);
            Start()
                .Elapsed.Ticks.Should().Be(0);

            Advance(345);
            Elapsed.Ticks.Should().Be(345);
        }

        [Fact]
        public void TestElapsedWhileNotRunning()
        {
            Advance(1);
            Start();

            Advance(4);
            Stop();

            Advance(9);
            Elapsed.Ticks.Should().Be(4);
        }

        [Fact]
        public void TestElapsedMultipleSegments()
        {
            Start();
            Advance(9);
            Stop();

            Advance(16);

            Start()
                .Elapsed.Ticks.Should().Be(9);

            Advance(25);
            Elapsed.Ticks.Should().Be(34);

            Stop();
            Advance(36);
            Elapsed.Ticks.Should().Be(34);
        }

        #region Test Helpers

        private TimeSpan Elapsed
        {
            get { return stopwatch.Elapsed; }
        }

        private bool IsRunning
        {
            get { return stopwatch.IsRunning; }
        }

        private void Advance(long amount)
        {
            ticker.Advance(amount);
        }

        private IStopwatch Reset()
        {
            return stopwatch.Reset();
        }

        private IStopwatch Start()
        {
            return stopwatch.Start();
        }

        private IStopwatch Stop()
        {
            return stopwatch.Stop();
        }

        #endregion
    }
}
