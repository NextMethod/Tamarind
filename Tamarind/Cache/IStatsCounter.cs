using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamarind.Cache
{
    /// <summary>
    /// Accumulates statistics during the operation of a <see cref="Cache"/> for presentation by 
    /// <see cref="IStatsCounter"/> . This is solely intended for consumption by <see cref="Cache"/> implementors.
    /// </summary>
    public interface IStatsCounter
    {
        void RecordHits(int count);
        void RecordMisses(int count);
        void RecordLoadSuccess(long loadTime);
        void RecordLoadException(long loadTime);
        void RecordEviction();
        ICacheStats Snapshot();
    }
}
