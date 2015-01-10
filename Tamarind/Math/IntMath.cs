using System;
using System.Linq;

using Tamarind.Core;
using Tamarind.Primitives;

namespace Tamarind.Math
{
    public static class IntMath
    {
        /// <summary>
        /// Returns <c>true</c> if <paramref name="x"/> represents a power of two.
        /// </summary>
        public static bool IsPowerOfTwo(int x)
        {
            return x > 0 & (x & (x - 1)) == 0;
        }

        /// <summary>
        ///     Returns the base-2 logarithms of <paramref name="x" />.
        /// </summary>
        public static int Log2(int x, bool ceiling = true)
        {
            Preconditions.CheckArgument(x > 0, "x", "must be > 0");

            if (ceiling)
            {
                return Ints.ByteCount - NumberOfLeadingZeros(x - 1);
            }

            return (Ints.ByteCount - 1) - NumberOfLeadingZeros(x);
        }

        // Taken from http://stackoverflow.com/a/24731095/27536
        public static int NumberOfLeadingZeros(int x)
        {
            x |= (x >> 1);
            x |= (x >> 2);
            x |= (x >> 4);
            x |= (x >> 8);
            x |= (x >> 16);
            return Ints.ByteCount - Ones(x);
        }

        private static int Ones(int x)
        {
            x -= ((x >> 1) & 0x55555555);
            x = (((x >> 2) & 0x33333333) + (x & 0x33333333));
            x = (((x >> 4) + x) & 0x0f0f0f0f);
            x += (x >> 8);
            x += (x >> 16);
            return (x & 0x0000003f);
        }

    }
}
