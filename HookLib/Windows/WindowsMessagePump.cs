using System;
using System.Windows.Interop;
using HookLib.Windows.Native;

namespace HookLib.Windows
{
    public class WindowsMessagePump
    {
        private const Int32 WM_QUIT = 0x12;

        public void Run()
        {
            var msg = new MSG();

            while (true)
            {

                if (!GetMessage(ref msg, IntPtr.Zero, 0, 0))
                {
                    break;
                }

                if (msg.message == WM_QUIT)
                    break;

                User32Native.TranslateMessage(ref msg);
                User32Native.DispatchMessage(ref msg);
            }
        }

        private bool GetMessage(ref MSG msg, IntPtr hwnd, int minMessage, int maxMessage)
        {
            return User32Native.GetMessage(ref msg, hwnd.ToInt32(), minMessage, maxMessage);
        }
    }
}
