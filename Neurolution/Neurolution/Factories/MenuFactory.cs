using Microsoft.Xna.Framework;
using Neurolution.Graphics;
using Neurolution.Managers;
using Neurolution.Menu;

namespace Neurolution.Factories
{
    //This factory creates menus and their elements (e.g. buttons)
    public class MenuFactory
    {
        private readonly GraphicsManager _graphicsManager;
        public MenuFactory(GraphicsManager graphicsManager)
        {
            _graphicsManager = graphicsManager;
        }

        public MenuButton CreateMenuButton(string name)
        {
            return new MenuButton(
                _graphicsManager.GetTexture("Button" + name),
                _graphicsManager.GetTexture("Button" + name + "Over")
                );
        }

        public IMenu CreateMainMenu(MenuPainter painter)
        {
            return new MainMenu(
                _graphicsManager.GetTexture("background"),
                painter,
                this
                );
        }

        public IMenu CreateGameMenu(MenuPainter painter)
        {
            return new GameMenu(
                painter.CreatePixel(Color.Black),
                painter,
                this
                );
        }
    }
}
