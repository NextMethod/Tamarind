using System;
using System.Linq;

using Tamarind.Base;
using Tamarind.Primitives;

namespace Tamarind.Threading
{
    internal class CompactStriped<T> : PowerOfTwoStriped<T>
    {

        private readonly T[] _array;

        public CompactStriped(int stripes, Func<T> supplier) : base(stripes)
        {
            Preconditions.CheckArgument(stripes <= Ints.MaxPowerOfTwo, "stripes", "must be <= 2^30");

            _array = new T[Mask + 1];
            for (var i = 0; i < _array.Length; i++)
            {
                _array[i] = supplier();
            }
        }

        public override T this[int index]
        {
            get { return _array[index]; }
        }

        public override int Size
        {
            get { return _array.Length; }
        }

    }
}
