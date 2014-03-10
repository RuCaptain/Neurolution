using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Neurolution.Managers;
using Neurolution.Menu;

namespace Neurolution.Graphics
{
    //Used for drawing a menu elements
    public class MenuPainter : Painter
    {
        public MenuPainter(GraphicsManager manager) : base(manager) { }

        public void DrawSimpleMenu(List<MenuItem> buttons, Texture2D background )
        {
            StartDrawing();

            GraphicsManager.DrawBackground(background);
            foreach (var button in buttons)
                DrawMenuButton(button);

            EndDrawing();
        }

        public void DrawMenuButton(MenuItem button)
        {
            GraphicsManager.DrawElement(button.CurrentTexture(), button.Position);
        }
    }
}
