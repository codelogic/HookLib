using System;
using System.Windows.Input;

namespace HookLib.Windows
{
    public class MouseHookEventArgs : MouseEventArgs
    {
        public MouseHookEventArgs(MouseDevice device, int timestamp)
            : base(device, timestamp)
        {
            // System.Windows.Input.InputManager.Current.PrimaryMouseDevice
            // NOTE: How or should I use an Input.InputManager.Current.PrimaryMouseDevice to figure out the current mouse device?
            // IS that device updated globaly? Does it throw off events? What if their are two devices, should I rely on simple input devices?
            // currently, the MouseDevice is a Dispatcher Object, which means its tied to a particular thread, which means that I probably can't
            // thread the hook manager and use LINQ to events the way I'd like to... Or should I just screw it all and add a dependency to win
            // forms? I really hate win forms, and having a dependency on it. If I can remove the dependency on a single mouse input device (IE
            // multi-user touch apps that need this kind of statistics tracking) and still retain the ability to filter and determin the exact
            // and full raw data, that becomes extreamly valuble later when I want to run statistics or add data processing hooks.
        }
    }
}