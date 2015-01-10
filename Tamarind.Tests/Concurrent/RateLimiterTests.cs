using System;

using FluentAssertions;

using System.Linq;
using System.Threading.Tasks;

using Tamarind.Concurrent;
using Tamarind.Core;
using Tamarind.Tests.Mocks;

using Xunit;

namespace Tamarind.Tests.Concurrent
{
    public class RateLimiterTests
    {

        private readonly FakeSleepingStopwatch stopwatch = new FakeSleepingStopwatch();

        [Fact]
        public async Task TestSimple()
        {
            var limiter = RateLimiter.Create(5.0, stopwatch);
            await limiter.Acquire();
            await limiter.Acquire();
            await limiter.Acquire();

            stopwatch.Events.Should()
                .ContainInOrder("R0.00", "R0.20", "R0.20");
        }

        [Fact]
        public async Task TestImmediateTryAcquire()
        {
            var limiter = RateLimiter.Create(1);
            (await limiter.TryAcquire()).Should().BeTrue();
            (await limiter.TryAcquire()).Should().BeFalse();
        }

        [Fact]
        public void TestSimpleRateUpdate()
        {
            var limiter = RateLimiter.Create(5.0, TimeSpan.FromSeconds(5));
            limiter.GetRate().Should().Be(5.0);

            limiter.SetRate(10.0);
            limiter.GetRate().Should().Be(10.0);

            limiter.Invoking(x => x.SetRate(0.0))
                .ShouldThrow<ArgumentException>();
            limiter.Invoking(x => x.SetRate(-10.0))
                .ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void TestAcquireParameterValidation()
        {
            var limiter = RateLimiter.Create(999);

            limiter.Invoking(x => x.Acquire(0).Wait())
                .ShouldThrow<ArgumentException>();

            limiter.Invoking(x => x.Acquire(-1).Wait())
                .ShouldThrow<ArgumentException>();

            limiter.Invoking(x => x.TryAcquire(0).Wait())
                .ShouldThrow<ArgumentException>();

            limiter.Invoking(x => x.TryAcquire(-1).Wait())
                .ShouldThrow<ArgumentException>();

            limiter.Invoking(x => x.TryAcquire(0, 1, TimeUnit.Seconds).Wait())
                .ShouldThrow<ArgumentException>();

            limiter.Invoking(x => x.TryAcquire(-1, 1, TimeUnit.Seconds).Wait())
                .ShouldThrow<ArgumentException>();
        }

        [Fact]
        public async Task TestSimpleWithWait()
        {
            var limiter = RateLimiter.Create(5.0, stopwatch);

            await limiter.Acquire(); // R0.00
            stopwatch.SleepMillis(200); // U0.20, we are read for the next request
            await limiter.Acquire(); // R0.00, granted immediately
            await limiter.Acquire(); // R0.20

            stopwatch.Events.Should()
                .ContainInOrder("R0.00", "U0.20", "R0.00", "R0.20");
        }

        [Fact]
        public async Task TestSimpleAcquireReturnValues()
        {
            var limiter = RateLimiter.Create(5.0, stopwatch);

            (await limiter.Acquire()).ShouldBeEquivalentTo(TimeSpan.Zero); // R0.00
            stopwatch.SleepMillis(200); // U0.20, we are ready for the next request
            (await limiter.Acquire()).ShouldBeEquivalentTo(TimeSpan.Zero);
            (await limiter.Acquire()).ShouldBeEquivalentTo(TimeSpan.FromSeconds(0.2)); // R0.20

            stopwatch.Events.Should()
                .ContainInOrder("R0.00", "U0.20", "R0.00", "R0.20");
        }

        [Fact]
        public async Task TestSimpleAcquireEarliestAvailableIsInPast()
        {
            var limiter = RateLimiter.Create(5.0, stopwatch);

            (await limiter.Acquire()).ShouldBeEquivalentTo(TimeSpan.Zero);
            stopwatch.SleepMillis(400);
            (await limiter.Acquire()).ShouldBeEquivalentTo(TimeSpan.Zero);
            (await limiter.Acquire()).ShouldBeEquivalentTo(TimeSpan.Zero);
            (await limiter.Acquire()).ShouldBeEquivalentTo(TimeSpan.FromSeconds(0.2)); // R0.20
        }

        [Fact]
        public async Task TestOneSecondBurst()
        {
            var limiter = RateLimiter.Create(5.0, stopwatch);
            stopwatch.SleepMillis(1000); // max capacity reached
            stopwatch.SleepMillis(1000); // this makes no difference

            (await limiter.Acquire()).ShouldBeEquivalentTo(TimeSpan.Zero, "because it's the first request"); // R0.00

            (await limiter.Acquire()).ShouldBeEquivalentTo(TimeSpan.Zero, "because it's from capacity");
            (await limiter.Acquire(3)).ShouldBeEquivalentTo(TimeSpan.Zero, "because it's from capacity");
            (await limiter.Acquire()).ShouldBeEquivalentTo(TimeSpan.Zero, "because it's concluding a burst of 5 permits");

            (await limiter.Acquire()).ShouldBeEquivalentTo(TimeSpan.FromSeconds(0.2), "because capacity is exhausted");

            stopwatch.Events.Should()
                .ContainInOrder(
                    "U1.00",
                    "U1.00",
                    "R0.00",
                    "R0.00",
                    "R0.00",
                    "R0.00",
                    "R0.20"
                );
        }

        [Fact]
        public void TestCreateWarmupParameterValidation()
        {
            RateLimiter.Create(1.0, TimeSpan.FromTicks(1));
            RateLimiter.Create(1.0, TimeSpan.Zero);

            Action action = () => RateLimiter.Create(0.0, TimeSpan.FromTicks(1));
            action.ShouldThrow<ArgumentException>();

            action = () => RateLimiter.Create(1.0, TimeSpan.FromTicks(-1));
            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public async Task TestWarmUp()
        {
            var limiter = RateLimiter.Create(2.0, TimeSpan.FromMilliseconds(4000), stopwatch);
            for (var i = 0; i < 8; i++)
            {
                await limiter.Acquire(); // #1
            }

            stopwatch.SleepMillis(500); // #2 to repay the last acquire
            stopwatch.SleepMillis(4000); // #3 becomes cold again

            for (var i = 0; i < 8; i++)
            {
                await limiter.Acquire(); // #4
            }

            stopwatch.SleepMillis(500); // #5 to repay the last acquire
            stopwatch.SleepMillis(2000); // #6 didn't get cold! it would take another 2 seconds to go cold

            for (var i = 0; i < 8; i++)
            {
                await limiter.Acquire(); // #7
            }


            stopwatch.Events.Should()
                .ContainInOrder(
                    "R0.00", "R1.38", "R1.13", "R0.88", "R0.63", "R0.50", "R0.50", "R0.50", // #1
                    "U0.50", // #2
                    "U4.00", // #3
                    "R0.00", "R1.38", "R1.13", "R0.88", "R0.63", "R0.50", "R0.50", "R0.50", // #4
                    "U0.50", // #5
                    "U2.00", // #6
                    "R0.00", "R0.50", "R0.50", "R0.50", "R0.50", "R0.50", "R0.50", "R0.50" // #7
                );
        }

        [Fact]
        public async Task TestWarmUpAndUpdate()
        {
            var limiter = RateLimiter.Create(2.0, TimeSpan.FromMilliseconds(4000), stopwatch);
            for (var i = 0; i < 8; i++)
            {
                await limiter.Acquire(); // #1
            }

            stopwatch.SleepMillis(4500); // #2 back to cold state (warmup period + repay last acquire)

            for (var i = 0; i < 3; i++)
            {
                await limiter.Acquire(); // #3
            }

            limiter.SetRate(4.0); // double the rate!
            await limiter.Acquire(); // #4, we repay the debt of the last acquire (imposed by the old rate)

            for (var i = 0; i < 4; i++)
            {
                await limiter.Acquire(); // #5
            }

            stopwatch.SleepMillis(4250); // #6, back to cold state (warmup period + repay last acquire)

            for (var i = 0; i < 11; i++)
            {
                await limiter.Acquire(); // #7, showing off the warmup starting from totally cold
            }

            // Make sure the areas (times) remain the same, while permits are different
            stopwatch.Events.Should()
                .ContainInOrder(
                    "R0.00", "R1.38", "R1.13", "R0.88", "R0.63", "R0.50", "R0.50", "R0.50", // #1
                    "U4.50", // #2
                    "R0.00", "R1.38", "R1.13", // #3, after that the rate changes
                    "R0.88", // #4, this is what the throttling would be with the old rate
                    "R0.34", "R0.28", "R0.25", "R0.25", // #5
                    "U4.25", // #6
                    "R0.00", "R0.72", "R0.66", "R0.59", "R0.53", "R0.47", "R0.41", // #7
                    "R0.34", "R0.28", "R0.25", "R0.25" // #7 (cont.), note, this matches #5
                );
        }

        [Fact]
        public async Task TestBurstyAndUpdate()
        {
            var limiter = RateLimiter.Create(1.0, stopwatch);

            await limiter.Acquire(); // no wait
            await limiter.Acquire(); // R1.00, to repay previous

            limiter.SetRate(2.0);
            
            await limiter.Acquire(); // R1.00, to repay previous (the previous was under the old rate)
            await limiter.Acquire(2); // R0.50, to repay previous (now the rate takes effect)
            await limiter.Acquire(4); // R1.00, to repay previous
            await limiter.Acquire(); // R2.00, to repay previous

            stopwatch.Events.Should()
                .ContainInOrder(
                "R0.00", "R1.00", "R1.00", "R0.50", "R1.00", "R2.00"
                );
        }

        [Fact]
        public async Task TestTryAcquireNoWaitAllowed()
        {
            var limiter = RateLimiter.Create(5.0, stopwatch);

            (await limiter.TryAcquire(unit: TimeUnit.Seconds)).Should().BeTrue();
            (await limiter.TryAcquire(unit: TimeUnit.Seconds)).Should().BeFalse();
            (await limiter.TryAcquire(unit: TimeUnit.Seconds)).Should().BeFalse();

            stopwatch.SleepMillis(100);

            (await limiter.TryAcquire(unit: TimeUnit.Seconds)).Should().BeFalse();
        }

        [Fact]
        public async Task TestTryAcquireSomeWaitAllowed()
        {
            var limiter = RateLimiter.Create(5.0, stopwatch);

            (await limiter.TryAcquire(unit: TimeUnit.Seconds))
                .Should().BeTrue();
            (await limiter.TryAcquire(timeout: 200, unit: TimeUnit.Milliseconds))
                .Should().BeTrue();
            (await limiter.TryAcquire(timeout: 100, unit: TimeUnit.Milliseconds))
                .Should().BeFalse();

            stopwatch.SleepMillis(100);

            (await limiter.TryAcquire(timeout: 100, unit: TimeUnit.Milliseconds))
                .Should().BeTrue();
        }

        [Fact]
        public async Task TestTryAcquireOverflow()
        {
            var limiter = RateLimiter.Create(5.0, stopwatch);

            (await limiter.TryAcquire(timeout: 0, unit: TimeUnit.Microseconds))
                .Should().BeTrue();

            stopwatch.SleepMillis(100);

            (await limiter.TryAcquire(timeout: long.MaxValue, unit: TimeUnit.Microseconds))
                .Should().BeTrue();
        }

        [Fact]
        public async Task TestTryAcquireNegative()
        {
            var limiter = RateLimiter.Create(5.0, stopwatch);

            (await limiter.TryAcquire(5, 0, TimeUnit.Seconds))
                .Should().BeTrue();

            stopwatch.SleepMillis(900);

            (await limiter.TryAcquire(1, long.MinValue, TimeUnit.Seconds))
                .Should().BeFalse();

            stopwatch.SleepMillis(100);

            (await limiter.TryAcquire(1, -1, TimeUnit.Seconds))
                .Should().BeTrue();
        }

        [Fact]
        public async Task TestSimpleWeights()
        {
            var limiter = RateLimiter.Create(1.0, stopwatch);
            await limiter.Acquire(); // no wait
            await limiter.Acquire(); // R1.00, to repay previous
            await limiter.Acquire(2); // R1.00, to repay previous
            await limiter.Acquire(4); // R2.00, to repay previous
            await limiter.Acquire(8); // R4.00, to repay previous
            await limiter.Acquire(1); // R8.00, to repay previous

            stopwatch.Events.Should()
                .ContainInOrder(
                "R0.00", "R1.00", "R1.00", "R2.00", "R4.00", "R8.00"
                );
        }

        [Fact]
        public async Task TestInfinityBursty()
        {
            var limiter = RateLimiter.Create(double.PositiveInfinity, stopwatch);
            await limiter.Acquire(int.MaxValue / 4);
            await limiter.Acquire(int.MaxValue / 2);
            await limiter.Acquire(int.MaxValue);

            stopwatch.Events.Should()
                .ContainInOrder(
                "R0.00", "R0.00", "R0.00" // no wait, infinite rate!
                );

            limiter.SetRate(2.0);
            await limiter.Acquire();
            await limiter.Acquire();
            await limiter.Acquire();
            await limiter.Acquire();
            await limiter.Acquire();

            stopwatch.Events.Should()
                .ContainInOrder(
                "R0.00", // First comes the saved-up burst, which defaults to a 1 second burst (2 requests).
                "R0.00", 
                "R0.00", // Now comes the free request
                "R0.50", // Now it's 0.5 seconds per request.
                "R0.50"
                );

            limiter.SetRate(double.PositiveInfinity);
            await limiter.Acquire();
            await limiter.Acquire();
            await limiter.Acquire();

            stopwatch.Events.Should()
                .ContainInOrder(
                "R0.50", "R0.00", "R0.00" // We repay the last request (.5sec), then back to +oo
                );
        }

        [Fact]
        public async Task TestInfinityBurstyTimeElapsed()
        {
            var limiter = RateLimiter.Create(double.PositiveInfinity, stopwatch);
            stopwatch.Instant += 1000000;
            limiter.SetRate(2.0);

            for (var i = 0; i < 5; i++)
            {
                await limiter.Acquire();
            }

            stopwatch.Events.Should()
                .ContainInOrder(
                "R0.00", // First comes the saved-up burst, which defaults to a 1 second burst (2 requests).
                "R0.00",
                "R0.00", // Now comes the free request.
                "R0.50", // Now it's 0.5 seconds per request.
                "R0.50"
                );
        }

        [Fact]
        public async Task TestInfinityWarmUp()
        {
            var limiter = RateLimiter.Create(double.PositiveInfinity, TimeSpan.FromSeconds(10), stopwatch);
            await limiter.Acquire(int.MaxValue / 4);
            await limiter.Acquire(int.MaxValue / 2);
            await limiter.Acquire(int.MaxValue);

            stopwatch.Events.Should()
                .ContainInOrder("R0.00", "R0.00", "R0.00");

            limiter.SetRate(1.0);

            await limiter.Acquire();
            await limiter.Acquire();
            await limiter.Acquire();

            stopwatch.Events.Should()
                .ContainInOrder("R0.00", "R1.00", "R1.00");

            limiter.SetRate(double.PositiveInfinity);

            await limiter.Acquire();
            await limiter.Acquire();
            await limiter.Acquire();

            stopwatch.Events.Should()
                .ContainInOrder("R1.00", "R0.00", "R0.00");

        }

        [Fact]
        public async Task TestInfinityWarmUpTimeElapsed()
        {
            var limiter = RateLimiter.Create(double.PositiveInfinity, TimeSpan.FromSeconds(10), stopwatch);
            stopwatch.Instant += 1000000;
            limiter.SetRate(1.0);

            for (var i = 0; i < 5; i++)
            {
                await limiter.Acquire();
            }

            stopwatch.Events.Should()
                .ContainInOrder("R0.00", "R1.00", "R1.00", "R1.00", "R1.00");
        }

    }
}
