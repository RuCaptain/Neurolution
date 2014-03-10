using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Neurolution.Controls;
using Neurolution.Factories;
using Neurolution.Graphics;
using Neurolution.Helpers;
using Neurolution.Menu;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Neurolution.Managers
{
    //Processes menus
    public class MenuManager : IManager
    {
        private readonly List<IMenu> _menus = new List<IMenu>();
        private IMenu _currentMenu;
        private IControlsObservable _controlsObservable;

        public void LoadContent(MenuPainter painter, MenuFactory factory)
        {
            _menus.Add(factory.CreateMainMenu(painter));
            _menus.Add(factory.CreateGameMenu(painter));
        }

        //Switching current menu to another
        public void SetMenu(int menuNumber)
        {
            if(_currentMenu != null) _controlsObservable.Unregister(_currentMenu);
            _currentMenu = _menus[menuNumber];
            _currentMenu.RegisterControls(_controlsObservable);
            Resize();
        }

        public void Leave()
        {
            if (_currentMenu != null) _controlsObservable.Unregister(_currentMenu);
        }

        public void Resize()
        {
            _currentMenu.Resize();
        }

        public void SetBackground(Texture2D background)
        {
            _currentMenu.SetBackground(background);
        }

        public void LoadContent(ControlsManager content)
        {
        }

        public void ControlsUpdated(Dictionary<Keys, KeyState> keyStates, Dictionary<MouseButtons, ButtonState> buttonStates, Point cursorPoint, int scroll)
        {
        }

        public void RegisterControls(IControlsObservable observable)
        {
            _controlsObservable = observable;
            SetMenu(0);
        }

        public void Initialize()
        {
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime)
        {
            _currentMenu.Draw(gameTime);

        }

        public string GetName()
        {
            return "menumanager";
        }

        public string CurrentMenuName()
        {
            return _currentMenu.GetName();
        }

        public void Register(IActionObserver observer)
        {
            foreach(var menu in _menus)
                menu.Register(observer);
        }
    }
}
