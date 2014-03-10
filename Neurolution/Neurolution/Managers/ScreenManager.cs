using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Neurolution.Controls;
using Neurolution.Graphics;
using Neurolution.Helpers;
using Neurolution.Screens;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Neurolution.Managers
{
    //Sets and processes the current state of program, using it's screens with specified purpose
    public class ScreenManager : IManager
    {
        private readonly List<IScreen> _screens = new List<IScreen>();
        private IScreen _currentScreen;
        private IControlsObservable _controlsObservable;

        public void SetScreen(int screenNumber, bool initialize)
        {
            if (_currentScreen != null) _controlsObservable.Unregister(_currentScreen);

            _currentScreen = _screens[screenNumber];
            _currentScreen.RegisterControls(_controlsObservable);

            if(initialize)_currentScreen.Initialize();
        }

        public void Draw(GameTime gameTime)
        {
            _currentScreen.Draw(gameTime);
        }

        public void Load(){}


        public void LoadContent(Camera2D camera)
        {
            _screens.Add(new MenuScreen());
            _screens.Add(new GameScreen(camera));
            SetScreen(0, true);
        }

        public void Initialize()
        {
        }

        public void Update(GameTime gameTime)
        {
            _currentScreen.Update(gameTime);
        }

        public string GetName()
        {
            return "screenmanager";
        }

        public void RegisterControls(IControlsObservable observable)
        {
            _controlsObservable = observable;
        }

        public void ControlsUpdated(Dictionary<Keys, KeyState> keyStates, Dictionary<MouseButtons, ButtonState> buttonStates, Point cursorPoint, int scroll)
        {
        }

        public void Register(IActionObserver observer)
        {
            foreach(var screen in _screens)
                screen.Register(observer);
        }
    }
}
