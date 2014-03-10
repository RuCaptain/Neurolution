using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Neurolution.Controls
{
    //This class stores links to ControlManager's listeners and their keys and mouse buttons
    public class ControlStateListener
    {
        public Dictionary<Keys, KeyState> KeyStates = new Dictionary<Keys, KeyState>();
        public Dictionary<MouseButtons, ButtonState> ButtonStates = new Dictionary<MouseButtons, ButtonState>();
        public Point CursorPoint = new Point();
        public readonly IControlsObserver Observer;
        private readonly bool _alwaysNotify;
        private int _scroll;
        private bool _doNotify;

        public ControlStateListener(IEnumerable<Keys> keys, IEnumerable<MouseButtons> buttons, bool alwaysNotify, IControlsObserver observer)
        {
            _alwaysNotify = alwaysNotify;
            foreach(var key in keys)
                KeyStates.Add(key, KeyState.Up);
            foreach (var button in buttons)
                ButtonStates.Add(button, ButtonState.Released);
            
            Observer = observer;
        }

        //Notifying listeners via IControlsObserver interface
        private void Notify()
        {
            Observer.ControlsUpdated(KeyStates, ButtonStates, CursorPoint, _scroll);
        }

        //Updating states (called from ControlsManager)
        public void UpdateKeyState(Keys key, KeyState state)
        {
            if (key == Keys.None || KeyStates[key] == state && !_alwaysNotify) return;

            KeyStates[key] = state;
            _doNotify = true;
        }

        public void UpdateButtonState(MouseButtons button, ButtonState state)
        {
            if (button == MouseButtons.None || ButtonStates[button] == state && !_alwaysNotify) return;


            ButtonStates[button] = state;
            _doNotify = true;
        }

        public void UpdateCursorPos(int mouseX, int mouseY)
        {
            if ((mouseX == CursorPoint.X || mouseY == CursorPoint.Y) && !_alwaysNotify) return;
            CursorPoint = new Point(mouseX, mouseY);
            _doNotify = true;
        }

        public void UpdateScroll(int value)
        {
            if (_scroll == value && !_alwaysNotify) return;
            _scroll = value;
        }

        //Notifying listeners if necessary
        public void Proceed()
        {
            if (!_doNotify) return;
            Notify();
            _doNotify = false;
        }

    }
}
