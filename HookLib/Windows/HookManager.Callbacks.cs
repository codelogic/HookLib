using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace HookLib.Windows
{
    public partial class HookManager
    {
        /// <summary>
        /// The CallWndProc hook procedure is an application-defined or library-defined callback 
        /// function used with the SetWindowsHookEx function. The HOOKPROC type defines a pointer 
        /// to this callback function. CallWndProc is a placeholder for the application-defined 
        /// or library-defined function name.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/callwndproc.asp
        /// </remarks>
        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        #region Windows Mouse Hooks

        /// <summary>
        /// This field is not objectively needed but we need to keep a reference on a delegate which will be 
        /// passed to unmanaged code. To avoid GC to clean it up.
        /// When passing delegates to unmanaged code, they must be kept alive by the managed application 
        /// until it is guaranteed that they will never be called.
        /// </summary>
        private HookProc _windowsMouseDelegate;

        /// <summary>
        /// Stores the handle to the mouse hook procedure.
        /// </summary>
        private int _windowsMouseHookHandle;

        private int _oldMouseX;
        private int _oldMouseY;

        /// <summary>
        /// A callback function which will be called every Time a mouse activity detected.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        private int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                //Marshall the data from callback.
                var mouseHookStruct = (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct));


                //detect button clicked
                //MouseHookEventArgs mouseArgs = new MouseHookEventArgs();
                short mouseDelta = 0;
                var clickCount = 0;
                var mouseDown = false;
                var mouseUp = false;

                switch (wParam)
                {
                    case WM_LBUTTONDOWN:
                        mouseDown = true;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONUP:
                        mouseUp = true;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONDBLCLK:
                        clickCount = 2;
                        break;
                    case WM_RBUTTONDOWN:
                        mouseDown = true;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONUP:
                        mouseUp = true;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONDBLCLK:
                        clickCount = 2;
                        break;
                    case WM_MOUSEWHEEL:
                        //If the message is WM_MOUSEWHEEL, the high-order word of MouseData member is the wheel delta. 
                        //One wheel click is defined as WHEEL_DELTA, which is 120. 
                        //(value >> 16) & 0xffff; retrieves the high-order word from the given 32-bit value
                        mouseDelta = (short)((mouseHookStruct.MouseData >> 16) & 0xffff);

                        //TODO: X BUTTONS (I havent them so was unable to test)
                        //If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP, 
                        //or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released, 
                        //and the low-order word is reserved. This value can be one or more of the following values. 
                        //Otherwise, MouseData is not used. 
                        break;
                }

                //generate event 
                var mouseEventArgs =
                    new GlobalMouseEventHandlerArgs(
                        new System.Windows.Point(mouseHookStruct.Point.X, mouseHookStruct.Point.Y),
                        mouseHookStruct.MouseData, mouseHookStruct.Flags, mouseHookStruct.Time,
                        mouseHookStruct.ExtraInfo);

                //Mouse up
                if (GlobalMouseUp != null && mouseUp)
                {
                    GlobalMouseUp.Invoke(null, mouseEventArgs);
                }

                //Mouse down
                if (GlobalMouseDown != null && mouseDown)
                {
                    GlobalMouseDown.Invoke(null, mouseEventArgs);
                }

                //If someone listens to click and a click occured
                if (GlobalMouseClick != null && clickCount > 0)
                {
                    GlobalMouseClick.Invoke(null, mouseEventArgs);
                }

                //If someone listens to double click and a click occured
                if (GlobalMouseDoubleClick != null && clickCount == 2)
                {
                    GlobalMouseDoubleClick.Invoke(null, mouseEventArgs);
                }

                //Wheel was moved
                if (GlobalMouseWheel != null && mouseDelta != 0)
                {
                    GlobalMouseWheel.Invoke(null, mouseEventArgs);
                }

                //If someone listens to move and there was a change in coordinates raise move event
                if ((GlobalMouseMove != null) && (_oldMouseX != mouseHookStruct.Point.X || _oldMouseY != mouseHookStruct.Point.Y))
                {
                    _oldMouseX = mouseHookStruct.Point.X;
                    _oldMouseY = mouseHookStruct.Point.Y;
                    if (GlobalMouseMove != null)
                    {
                        GlobalMouseMove.Invoke(null, mouseEventArgs);
                    }
                }

                if (mouseEventArgs.Handled)
                {
                    return -1;
                }
            }

            //call next hook
            return CallNextHookEx(_windowsMouseHookHandle, nCode, wParam, lParam);
        }

        private void EnsureSubscribedToGlobalMouseEvents()
        {
            // install Mouse hook only if it is not installed and must be installed
            if (_windowsMouseHookHandle == 0)
            {
                //See comment of this field. To avoid GC to clean it up.
                _windowsMouseDelegate = MouseHookProc;
                //install hook
                _windowsMouseHookHandle = SetWindowsHookEx(
                    WH_MOUSE_LL,
                    _windowsMouseDelegate,
                    Marshal.GetHINSTANCE(
                        Assembly.GetExecutingAssembly().GetModules()[0]),
                    0);
                //If SetWindowsHookEx fails.
                if (_windowsMouseHookHandle == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //do cleanup

                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        private void TryUnsubscribeFromGlobalMouseEvents()
        {
            //if no subsribers are registered unsubsribe from hook
            if (GlobalMouseClick == null &&
                GlobalMouseDown == null &&
                GlobalMouseMove == null &&
                GlobalMouseUp == null &&
                GlobalMouseWheel == null)
            {
                ForceUnsunscribeFromGlobalMouseEvents();
            }
        }

        private void ForceUnsunscribeFromGlobalMouseEvents()
        {
            if (_windowsMouseHookHandle != 0)
            {
                //uninstall hook
                var result = UnhookWindowsHookEx(_windowsMouseHookHandle);
                
                //reset invalid handle
                _windowsMouseHookHandle = 0;
                //Free up for GC
                _windowsMouseDelegate = null;
                //if failed and exception must be thrown
                if (result == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        #endregion

        #region Windows Keyboard Hooks

        /// <summary>
        /// This field is not objectively needed but we need to keep a reference on a delegate which will be 
        /// passed to unmanaged code. To avoid GC to clean it up.
        /// When passing delegates to unmanaged code, they must be kept alive by the managed application 
        /// until it is guaranteed that they will never be called.
        /// </summary>
        private HookProc _windowsKeyboardDelegate;

        /// <summary>
        /// Stores the handle to the Keyboard hook procedure.
        /// </summary>
        private int _windowsKeyboardHookHandle;

        /// <summary>
        /// A callback function which will be called every Time a keyboard activity detected.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.l
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //indicates if any of underlaing events set e.Handled flag
            var handled = false;

            if (nCode >= 0)
            {
                //read structure KeyboardHookStruct at lParam
                var keyStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));

                //raise KeyDown
                if (GlobalKeyDown != null && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    var keyEventArgs = new GlobalKeyEventHandlerArgs(
                        keyStruct.VirtualKeyCode, 
                        keyStruct.ScanCode,
                        keyStruct.Flags, 
                        keyStruct.Time, 
                        keyStruct.ExtraInfo,
                        null);

                    GlobalKeyDown.Invoke(null, keyEventArgs);
                    handled = keyEventArgs.Handled;
                }

                // raise KeyPress
                if (GlobalKeyPressed != null && wParam == WM_KEYDOWN)
                {
                    var isDownShift = ((GetKeyState(VK_SHIFT) & 0x80) == 0x80 ? true : false);
                    var isDownCapslock = (GetKeyState(VK_CAPITAL) != 0 ? true : false);

                    var keyState = new byte[256];
                    GetKeyboardState(keyState);
                    var inBuffer = new byte[2];

                    if (ToAscii(keyStruct.VirtualKeyCode,
                              keyStruct.ScanCode,
                              keyState,
                              inBuffer,
                              keyStruct.Flags) == 1)
                    {
                        var key = (char)inBuffer[0];
                        if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key)) key = Char.ToUpper(key);
                        
                        var keyEventArgs = new GlobalKeyEventHandlerArgs(
                            keyStruct.VirtualKeyCode, 
                            keyStruct.ScanCode,
                            keyStruct.Flags, 
                            keyStruct.Time, 
                            keyStruct.ExtraInfo,
                            key);

                        GlobalKeyPressed.Invoke(null, keyEventArgs);
                        handled = handled || keyEventArgs.Handled;
                    }
                }

                // raise KeyUp
                if (GlobalKeyUp != null && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    var keyEventArgs = new GlobalKeyEventHandlerArgs(
                            keyStruct.VirtualKeyCode,
                            keyStruct.ScanCode,
                            keyStruct.Flags,
                            keyStruct.Time,
                            keyStruct.ExtraInfo,
                            null);

                    GlobalKeyUp.Invoke(null, keyEventArgs);
                    handled = handled || keyEventArgs.Handled;
                }
            }

            //if event handled in application do not handoff to other listeners
            if (handled)
                return -1;

            //forward to other application
            return CallNextHookEx(_windowsKeyboardHookHandle, nCode, wParam, lParam);
        }

        private void EnsureSubscribedToGlobalKeyboardEvents()
        {
            // install Keyboard hook only if it is not installed and must be installed
            if (_windowsKeyboardHookHandle == 0)
            {
                //See comment of this field. To avoid GC to clean it up.
                _windowsKeyboardDelegate = KeyboardHookProc;
                //install hook
                _windowsKeyboardHookHandle = SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    _windowsKeyboardDelegate,
                    Marshal.GetHINSTANCE(
                        Assembly.GetExecutingAssembly().GetModules()[0]),
                    0);

                //If SetWindowsHookEx fails.
                if (_windowsKeyboardHookHandle == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    var errorCode = Marshal.GetLastWin32Error();

                    //do cleanup
                    ForceUnsunscribeFromGlobalKeyboardEvents();
                    ForceUnsunscribeFromGlobalMouseEvents();

                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        private void TryUnsubscribeFromGlobalKeyboardEvents()
        {
            //if no subsribers are registered unsubsribe from hook
            if (GlobalKeyDown == null &&
                GlobalKeyUp == null &&
                GlobalKeyPressed == null)
            {
                ForceUnsunscribeFromGlobalKeyboardEvents();
            }
        }

        private void ForceUnsunscribeFromGlobalKeyboardEvents()
        {
            if (_windowsKeyboardHookHandle != 0)
            {
                //uninstall hook
                var result = UnhookWindowsHookEx(_windowsKeyboardHookHandle);
                //reset invalid handle
                _windowsKeyboardHookHandle = 0;
                //Free up for GC
                _windowsKeyboardDelegate = null;
                //if failed and exception must be thrown
                if (result == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    var errorCode = Marshal.GetLastWin32Error();
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        #endregion
    }
}
