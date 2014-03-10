using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Neurolution.Controls;
using Neurolution.Factories;
using Neurolution.Graphics;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Neurolution.Menu
{
    public class MainMenu : SimpleMenu
    {

        public MainMenu(Texture2D background, MenuPainter painter, MenuFactory factory) : base(background, painter, factory)
        {
            var startButton = Factory.CreateMenuButton("NewSimulation");
            var exitButton = Factory.CreateMenuButton("Exit");
            
            startButton.Click += StartButtonOnClick;
            exitButton.Click += ExitButtonOnClick;
            Elements.Add(startButton);
            Elements.Add(exitButton);
            Resize();
        }

        public void ExitButtonOnClick(object sender, EventArgs eventArgs)
        {
            Observer.RequestAction("exit", new List<object>());
        }

        private void StartButtonOnClick(object sender, EventArgs eventArgs)
        {
            Observer.RequestAction("newgame", new List<object>());
        }

        public override void ControlsUpdated(Dictionary<Keys, KeyState> keyStates, Dictionary<MouseButtons, ButtonState> buttonStates, Point cursorPoint, int scroll)
        {
            ProcessButtons(buttonStates[MouseButtons.Left], cursorPoint);
        }

        public override void RegisterControls(IControlsObservable observable)
        {
            observable.Register(
                this,
                new List<Keys>(),
                new List<MouseButtons> { MouseButtons.Left });
        }

        public override string GetName()
        {
            return "mainmenu";
        }
    }
}
