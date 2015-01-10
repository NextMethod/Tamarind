using System;
using System.Linq;

namespace Tamarind.Base
{
    public class ActionDisposable : IDisposable
    {

        private readonly Action callback;

        public ActionDisposable(Action callback)
        {
            if (callback == null)
            {
                callback = () => { };
            }
            this.callback = callback;
        }

        public void Dispose()
        {
            callback();
        }

    }
}
