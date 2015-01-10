using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Tamarind.Concurrent;

using Xunit;

namespace Tamarind.Tests.Concurrent
{
    public class StripedTests
    {

        [Fact]
        private void TestSizes()
        {
            Striped.Lock(100).Size
                   .Should().BeGreaterOrEqualTo(100);
            Striped.Lock(256).Size
                   .Should().Be(256);
        }

        [Fact]
        private void TestBasicInvariants()
        {
            AssertBasicInvariants(Striped.Lock(100));
            AssertBasicInvariants(Striped.Lock(256));
            AssertBasicInvariants(Striped.Semaphore(100, 1));
            AssertBasicInvariants(Striped.Semaphore(256, 1));
            AssertBasicInvariants(Striped.ReaderWriterLock(100));
            AssertBasicInvariants(Striped.ReaderWriterLock(256));
        }

        private static void AssertBasicInvariants<T>(IStriped<T> striped)
        {
            var observed = new HashSet<object>();

            for (var i = 0; i < striped.Size; i++)
            {
                var @lock = striped[i];
                @lock.Should()
                     .NotBeNull();
                @lock.Should()
                     .BeSameAs(striped[i]);

                observed.Add(@lock);
            }

            observed.Count.Should()
                    .Be(striped.Size);


            // this uses Get(key), makes sure an already observed stripe is returned
            for (var i = 0; i < striped.Size * 100; i++)
            {
                observed.Contains(striped.Get(new object())).Should()
                        .BeTrue();
            }

            Action act = () =>
            {
                var ignored = striped[-1];
            };

            act.ShouldThrow<IndexOutOfRangeException>();


            act = () =>
            {
                var ignored = striped[striped.Size];
            };

            act.ShouldThrow<IndexOutOfRangeException>();
        }

    }
}
