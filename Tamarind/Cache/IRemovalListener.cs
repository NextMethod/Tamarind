using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamarind.Cache
{
    public interface IRemovalListener<TKey, TValue>
    {
        void onRemoval(RemovalNotification<TKey, TValue> notification);
    }
}
