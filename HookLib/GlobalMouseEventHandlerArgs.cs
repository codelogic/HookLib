using System;
using System.Windows;

namespace HookLib
{
    public class GlobalMouseEventHandlerArgs : HandleableEventArgs
    {
        public Point Point { get; private set; }
        public int MouseData { get; private set; }
        public int Flags { get; private set; }
        public int Time { get; private set; }
        public int ExtraInfo { get; private set; }

        public GlobalMouseEventHandlerArgs(Point point, int mouseData, int flags, int time, int extraInfo)
        {
            Point = point;
            MouseData = mouseData;
            Flags = flags;
            Time = time;
            ExtraInfo = extraInfo;
        }
    }
}