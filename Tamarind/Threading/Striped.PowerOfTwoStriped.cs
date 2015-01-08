using System;
using System.Linq;

using Tamarind.Base;
using Tamarind.Primitives;

namespace Tamarind.Threading
{
    internal abstract class PowerOfTwoStriped<T> : IStriped<T>
    {

        private readonly int _mask;

        protected PowerOfTwoStriped(int stripes)
        {
            Preconditions.CheckArgument(stripes > 0, "Stripes must be positive");
            _mask = stripes > Ints.MaxPowerOfTwo ? Striped.AllBitsSet : Striped.CeilToPowerOfTwo(stripes) - 1;
        }


        protected int Mask
        {
            get { return _mask; }
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
            return hash & _mask;
        }

    }
}
