using System;
using System.Linq;

namespace Tamarind.Base
{
    public class ActionDisposable : IDisposable
    {

        private readonly Action _callback;

        public ActionDisposable(Action callback)
        {
            _callback = callback;
        }


        public void Dispose()
        {
            _callback();
        }

    }
}
