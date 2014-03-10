using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Neurolution.Controls;
using Neurolution.Helpers;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Neurolution.Managers
{
    //Processes user inputs
    public class ControlsManager : IManager, IControlsObservable
    {
        private readonly List<ControlStateListener> _listeners = new List<ControlStateListener>();
        private int _lastScroll;
        private bool _listenersUpdated;
        private bool _altEnterToRelease;
        private bool _f12ToRelease;
        private IActionObserver _observer;

        //Registering listeners
        public void Register(IControlsObserver observer, List<Keys> keys, List<MouseButtons> buttons, bool alwaysNotify = false)
        {
            _listeners.Add(new ControlStateListener(
                keys,
                buttons,
                alwaysNotify,
                observer));
            _listenersUpdated = true;
        }

        public void Unregister(IControlsObserver observer)
        {
            _listeners.RemoveAll(p => p.Observer == observer);
            _listenersUpdated = true;
        }


        public void Initialize()
        {
        }

        public void Update(GameTime gameTime)
        {
            var scroll = Mouse.GetState().ScrollWheelValue;

            foreach (var listener in _listeners)
            {
                var keys = new List<Keys>(listener.KeyStates.Keys);
                foreach (var key in keys)
                {
                    var state = Keyboard.GetState(0).IsKeyDown(key) ? KeyState.Down : KeyState.Up;
                    listener.UpdateKeyState(key, state);
                }

                var buttons = new List<MouseButtons>(listener.ButtonStates.Keys);
                foreach (var button in buttons)
                {
                    ButtonState state;
                    switch (button)
                    {
                        case MouseButtons.Left:
                            state = Mouse.GetState().LeftButton;
                            break;
                        case MouseButtons.Right:
                            state = Mouse.GetState().RightButton;
                            break;
                        default:
                            state = ButtonState.Released;
                            break;
                    }
                    listener.UpdateButtonState(button, state);
                }

                var mouseX = Mouse.GetState().X;
                var mouseY = Mouse.GetState().Y;

                listener.UpdateCursorPos(mouseX, mouseY);
                listener.UpdateScroll(scroll - _lastScroll);
                listener.Proceed();
                if (_listenersUpdated) break;

            }

            _lastScroll = scroll;
            _listenersUpdated = false;
        }

        public void Draw(GameTime gameTime)
        {
        }


        public void LoadContent(ControlsManager content)
        {
        }


        //Processing global controls (fullscreen and screenshot keys)
        public void ControlsUpdated(Dictionary<Keys, KeyState> keyStates, Dictionary<MouseButtons, ButtonState> buttonStates, Point cursorPoint, int scroll)
        {

            var altPressed = keyStates[Keys.LeftAlt] == KeyState.Down || keyStates[Keys.RightAlt] == KeyState.Down;
            var enterPressed = keyStates[Keys.Enter] == KeyState.Down;

            if (altPressed && enterPressed && !_altEnterToRelease)
            {
                _observer.RequestAction("fullscreen", new List<object>());
                _altEnterToRelease = true;
            }
            else if (!(altPressed || enterPressed))
                _altEnterToRelease = false;

            var f12Pressed = keyStates[Keys.F12] == KeyState.Down;
            if (f12Pressed && !_f12ToRelease)
            {
                _observer.RequestAction("screenshot", new List<object>());
                _f12ToRelease = true;
            }
            else if (!f12Pressed)
                _f12ToRelease = false;
        }

        public void RegisterControls(IControlsObservable observable)
        {
            Register(this, new List<Keys> { Keys.LeftAlt, Keys.RightAlt, Keys.Enter, Keys.F12 }, new List<MouseButtons>());
        }

        public void LoadContent(ContentManager content)
        {
        }

        public string GetName()
        {
            return "controlsmanager";
        }

        public void Register(IActionObserver observer)
        {
            _observer = observer;
        }
    }
}
