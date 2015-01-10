using System;
using System.Linq;
using System.Threading;

using Tamarind.Annotations;
using Tamarind.Math;

namespace Tamarind.Concurrent
{
    public static class Striped
    {

        /// <summary>
        ///     Creates a <see cref="IStriped{SimpleLock}" /> with eagerly initialized, strongly referenced locks.
        /// </summary>
        /// <param name="stripes">The minimum number of stripes (locks) required</param>
        /// <returns>A new <c cref="IStriped{SimpleLock}" /></returns>
        [PublicAPI]
        public static IStriped<MonitorLock> Lock(int stripes)
        {
            return new CompactStriped<MonitorLock>(stripes, () => new MonitorLock());
        }

        /// <summary>
        ///     Creates a <see cref="IStriped{SemaphoreSlim}" /> with eagerly initialized, strongly referenced semaphores,
        ///     with the specified number of <paramref name="permits" />
        /// </summary>
        /// <param name="stripes">The minimum number of stripes (semaphores) required</param>
        /// <param name="permits">The number of permits in each semaphore</param>
        /// <param name="maxPermits">The maximum number of permits in each semaphore</param>
        /// <returns>A new <c cref="IStriped{SemaphoreSlim}" /></returns>
        [PublicAPI]
        public static IStriped<SemaphoreSlim> Semaphore(int stripes, int permits, int maxPermits = int.MaxValue)
        {
            return new CompactStriped<SemaphoreSlim>(stripes, () => new SemaphoreSlim(permits, maxPermits));
        }

        /// <summary>
        ///     Creates a <see cref="IStriped{ReaderWriterLockSlim}" /> with eagerly initialized, strongly referenced
        ///     read-write locks.
        /// </summary>
        /// <param name="stripes">The minimum number of stripes (read-write locks) required</param>
        /// <returns>A new <c cref="IStriped{ReaderWriterLockSlim}" /></returns>
        [PublicAPI]
        public static IStriped<ReaderWriterLockSlim> ReaderWriterLock(int stripes)
        {
            return new CompactStriped<ReaderWriterLockSlim>(stripes, () => new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion));
        }


        #region Private Bits

        /// <summary>
        /// A bit mask where all bits are set.
        /// </summary>
        internal const int AllBitsSet = ~0;

        internal static int CeilToPowerOfTwo(int x)
        {
            return 1 << IntMath.Log2(x);
        }

        internal static uint Smear(int hashCode)
        {
            var uhash = Convert.ToUInt32(hashCode);
            uhash ^= (uhash >> 20) ^ (uhash >> 12);
            return uhash ^ (uhash >> 7) ^ (uhash >> 4);
        }

        #endregion

    }
}
