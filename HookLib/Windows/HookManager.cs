using System;
using System.Windows.Input;

namespace HookLib.Windows
{
    /// <summary>
    /// Provides events and methods for easily managing global input events regardless of application focus.
    /// </summary>
    public partial class HookManager
    {
        #region Mouse events

        private event GlobalMouseEventHandler GlobalMouseMove;
        public event GlobalMouseEventHandler MouseMove
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                GlobalMouseMove += value;
            }
            remove
            {
                GlobalMouseMove -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private event GlobalMouseEventHandler GlobalMouseClick;
        public event GlobalMouseEventHandler MouseClick
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                GlobalMouseClick += value;
            }
            remove
            {
                GlobalMouseClick -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private event GlobalMouseEventHandler GlobalMouseDoubleClick;
        public event GlobalMouseEventHandler MouseDoubleClick
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                GlobalMouseDoubleClick += value;
            }
            remove
            {
                GlobalMouseDoubleClick -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private event GlobalMouseEventHandler GlobalMouseDown;
        public event GlobalMouseEventHandler MouseDown
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                GlobalMouseDown += value;
            }
            remove
            {
                GlobalMouseDown -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private event GlobalMouseEventHandler GlobalMouseUp;
        public event GlobalMouseEventHandler MouseUp
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                GlobalMouseUp += value;
            }
            remove
            {
                GlobalMouseUp -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private event GlobalMouseEventHandler GlobalMouseWheel;
        public event GlobalMouseEventHandler MouseWheel
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                GlobalMouseWheel += value;
            }
            remove
            {
                GlobalMouseWheel -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        #endregion

        #region Keyboard Events

        private event GlobalKeyEventHandler GlobalKeyPressed;
        public event GlobalKeyEventHandler KeyPressed
        {
            add
            {
                EnsureSubscribedToGlobalKeyboardEvents();
                GlobalKeyPressed += value;
            }
            remove
            {
                GlobalKeyPressed -= value;
                TryUnsubscribeFromGlobalKeyboardEvents();
            }
        }

        private event GlobalKeyEventHandler GlobalKeyUp;
        public event GlobalKeyEventHandler KeyUp
        {
            add
            {
                EnsureSubscribedToGlobalKeyboardEvents();
                GlobalKeyUp += value;
            }
            remove
            {
                GlobalKeyUp -= value;
                TryUnsubscribeFromGlobalKeyboardEvents();
            }
        }

        private event GlobalKeyEventHandler GlobalKeyDown;
        public event GlobalKeyEventHandler KeyDown
        {
            add
            {
                EnsureSubscribedToGlobalKeyboardEvents();
                GlobalKeyDown += value;
            }
            remove
            {
                GlobalKeyDown -= value;
                TryUnsubscribeFromGlobalKeyboardEvents();
            }
        }

        #endregion
    }
}