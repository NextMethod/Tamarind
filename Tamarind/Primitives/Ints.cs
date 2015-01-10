using System;
using System.Linq;

using Tamarind.Annotations;

namespace Tamarind.Primitives
{
    public static class Ints
    {
        
        /// <summary>
        /// Number of bits used to represent an <see cref="int" /> value in two's complement binary form.
        /// </summary>
        [PublicAPI]
        public const int BitCount = ByteCount * Bytes.BitCount;
        
        /// <summary>
        /// Number of bytes used to represent an <see cref="int" /> value in two's complement binary form.
        /// </summary>
        [PublicAPI]
        public const int ByteCount = sizeof (int);

        /// <summary>
        /// The largest power of two that can be represented as an <see cref="int" />
        /// </summary>
        [PublicAPI]
        public const int MaxPowerOfTwo = 1 << (BitCount - 2);

    }
}
