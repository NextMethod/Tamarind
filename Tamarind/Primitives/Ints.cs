using System;
using System.Linq;

using Tamarind.Annotations;

namespace Tamarind.Primitives
{
    public static class Ints
    {

        [PublicAPI]
        public static readonly int Size = sizeof (int) * 8;
        
        [PublicAPI]
        public static readonly int Bytes = Size / Primitives.Bytes.Size;
        
        [PublicAPI]
        public static readonly int MaxPowerOfTwo = 1 << (Size - 2);

    }
}
