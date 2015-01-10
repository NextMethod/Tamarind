using System;
using System.Linq;

using Tamarind.Core;
using Tamarind.Primitives;

namespace Tamarind.Concurrent
{
    internal abstract class PowerOfTwoStriped<T> : IStriped<T>
    {

        private readonly int mask;

        protected PowerOfTwoStriped(int stripes)
        {
            Preconditions.CheckArgument(stripes > 0, "Stripes must be positive");
            mask = stripes > Ints.MaxPowerOfTwo ? Striped.AllBitsSet : Striped.CeilToPowerOfTwo(stripes) - 1;
        }


        protected int Mask
        {
            get { return mask; }
        }

        public abstract T this[int index] { get; }

        public abstract int Size { get; }

        public T Get(object key)
        {
            return this[IndexOf(key)];
        }

        protected int IndexOf(object key)
        {
            var uhash = Striped.Smear(key.GetHashCode());
            var hash = Convert.ToInt32(uhash);
            return hash & mask;
        }

    }
}
