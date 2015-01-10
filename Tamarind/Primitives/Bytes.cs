using System;
using System.Linq;

using Tamarind.Annotations;

namespace Tamarind.Primitives
{
    public static class Bytes
    {

        /// <summary>
        /// Number of bits used to represent a <see cref="byte" /> value in two's complement binary form.
        /// </summary>
        [PublicAPI]
        public const int BitCount = ByteCount * 8;

        /// <summary>
        /// Number of bytes used to represent a <see cref="byte" /> value in two's complement binary form.
        /// </summary>
        [PublicAPI]
        public const int ByteCount = sizeof (byte);

    }
}
