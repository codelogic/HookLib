using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Interop;

namespace HookLib.Windows.Native
{
    internal static class User32Native
    {
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool TranslateMessage([In, Out] ref MSG msg);

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr DispatchMessage([In] ref MSG msg);

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetMessage(ref MSG lpMsg, Int32 hwnd, Int32 wMsgFilterMin, Int32 wMsgFilterMax);

    }
}
