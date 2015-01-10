using System;
using System.Linq;

// ReSharper disable once CheckNamespace

namespace Tamarind.Annotations
{

    /// <summary>
    ///     This attribute is intended to mark publicly available API
    ///     which should not be removed and so is treated as used
    /// </summary>
    [MeansImplicitUse]
    public sealed class VisibleForTestingAttribute : Attribute
    {

        public VisibleForTestingAttribute() {}

        public VisibleForTestingAttribute([NotNull] string comment)
        {
            Comment = comment;
        }

        [NotNull]
        public string Comment { get; private set; }

    }
}
