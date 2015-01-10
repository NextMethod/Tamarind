using System;
using System.Linq;

using Tamarind.Core;
using Tamarind.Primitives;

namespace Tamarind.Concurrent
{
    internal class CompactStriped<T> : PowerOfTwoStriped<T>
    {

        private readonly T[] array;

        public CompactStriped(int stripes, Func<T> supplier) : base(stripes)
        {
            Preconditions.CheckArgument(stripes <= Ints.MaxPowerOfTwo, "stripes", "must be <= 2^30");

            array = new T[Mask + 1];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = supplier();
            }
        }

        public override T this[int index]
        {
            get { return array[index]; }
        }

        public override int Size
        {
            get { return array.Length; }
        }

    }
}
