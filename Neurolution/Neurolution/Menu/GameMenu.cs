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
    //Game menu, which is called from game
    public class GameMenu : SimpleMenu
    {
        private bool _escToRelease;

        public GameMenu(Texture2D background, MenuPainter painter, MenuFactory factory)
            : base(background, painter, factory)
        {
            var returnButton = Factory.CreateMenuButton("Return");
            var startButton = Factory.CreateMenuButton("NewSimulation");
            var exitButton = Factory.CreateMenuButton("Exit");
            
            returnButton.Click += ReturnButtonOnClick;
            startButton.Click += StartButtonOnClick;
            exitButton.Click += ExitButtonOnClick;
            Elements.Add(returnButton);
            Elements.Add(startButton);
            Elements.Add(exitButton);

            Resize();
        }


        public void ReturnButtonOnClick(object sender, EventArgs eventArgs)
        {
            Observer.RequestAction("resume", new List<object>());
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

            if (keyStates[Keys.Escape] == KeyState.Down && !_escToRelease)
            {
                ReturnButtonOnClick(this, new EventArgs());
                _escToRelease = true;
            }
            else if (keyStates[Keys.Escape] == KeyState.Up)
                _escToRelease = false;
        }

        public override void RegisterControls(IControlsObservable observable)
        {
            observable.Register(
                this,
                new List<Keys>{Keys.Escape},
                new List<MouseButtons> { MouseButtons.Left });
        }
        
        public override string GetName()
        {
            return "gamemenu";
        }

        

    }
}
