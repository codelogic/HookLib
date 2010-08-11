using System;

namespace HookLib
{
    public class HandleableEventArgs : EventArgs
    {
        public bool Handled { get; set; }
    }
}