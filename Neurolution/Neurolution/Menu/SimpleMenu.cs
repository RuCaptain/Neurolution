using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Neurolution.Controls;
using Neurolution.Factories;
using Neurolution.Graphics;
using Neurolution.Helpers;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Neurolution.Menu
{
    //Simple menu is an menu with static background and buttons
    public abstract class SimpleMenu : IMenu
    {
        public List<MenuItem> Elements = new List<MenuItem>();
        protected Texture2D Background;
        protected IActionObserver Observer;
        protected readonly MenuPainter Painter;
        protected readonly MenuFactory Factory;
        protected bool MouseButtonToRelease;

        protected SimpleMenu(Texture2D background, MenuPainter painter, MenuFactory factory)
        {
            Factory = factory;
            Painter = painter;
            SetBackground(background);
        }


        public void SetBackground(Texture2D texture)
        {
            Background = texture;
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime)
        {
            Painter.DrawSimpleMenu(Elements, Background);
        }

        public void Resize()
        {
            if(Observer != null)Observer.RequestAction("menubackground", new List<object>());

            var buttonHeight = Elements[0].GetBounds().Height;
            var position = (float)Painter.GetBounds().Height / 2 - buttonHeight * (0.5f + 1.5f * (Elements.Count - 2));
            foreach (var button in Elements)
            {
                button.Position = new Vector2((float)Painter.GetBounds().Width / 2, position);
                position += buttonHeight * 2;
            }
        }

        public void Register(IActionObserver observer)
        {
            Observer = observer;
        }

        public void ProcessButtons(ButtonState mouseButtonState, Point cursorPoint)
        {

            foreach (var button in Elements)
                if (Utils.CursorInArea(cursorPoint, button.GetBounds()))
                {
                    if (mouseButtonState == ButtonState.Pressed && !MouseButtonToRelease)
                    {
                        button.DoClick();
                        MouseButtonToRelease = true;
                    }
                    else if (mouseButtonState == ButtonState.Released)
                        MouseButtonToRelease = false;

                    button.PerformAction("mouseover", new List<object>{ true });
                }
                else button.PerformAction("mouseover", new List<object> { false });
        }

        public abstract void ControlsUpdated(Dictionary<Keys, KeyState> keyStates,
            Dictionary<MouseButtons, ButtonState> buttonStates, Point cursorPoint, int scroll);

        public abstract void RegisterControls(IControlsObservable observable);

        public abstract string GetName();
        public void PerformAction(string action, List<object> parameters)
        {
        }

        public Dictionary<string, object> GetCustomData()
        {
            return new Dictionary<string, object>();
        }
    }
}
