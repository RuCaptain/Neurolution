using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Neurolution.Controls;
using Neurolution.Graphics;
using Neurolution.Helpers;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Neurolution.Screens
{
    //Game screen displays and controls the world
    public class GameScreen : IScreen
    {
        private IActionObserver _observer;
        private readonly Camera2D _camera;
        private bool _escToRelease;
        private readonly float _zoomMin;
        private readonly float _zoomMax;

        public GameScreen(Camera2D camera)
        {
            _camera = camera;
            _zoomMin = _camera.Zoom;
            _zoomMax = _camera.Zoom*8f;
        }

        public string GetName()
        {
            return "gamescreen";
        }

        public void PerformAction(string action, List<object> parameters)
        {
        }

        public Dictionary<string, object> GetCustomData()
        {
            return new Dictionary<string, object>();
        }

        //Let WorldManager does all work.

        public void Initialize()
        {
            _observer.RequestAction("init", new List<object>
            {
                "worldmanager"
            });
        }

        public void Update(GameTime gameTime)
        {
            _observer.RequestAction("update", new List<object>
            {
                "worldmanager"
            });
        }

        public void Draw(GameTime gameTime)
        {
            _observer.RequestAction("draw", new List<object>
            {
                "worldmanager"
            });
        }


        public void Pause()
        {
        }


        public void Register(IActionObserver observer)
        {
            _observer = observer;
        }


        //Processing game controls
        public void ControlsUpdated(Dictionary<Keys, KeyState> keyStates, Dictionary<MouseButtons, ButtonState> buttonStates, Point cursorPoint, int scroll)
        {
            if (keyStates[Keys.Escape] == KeyState.Down && !_escToRelease)
            {
                _escToRelease = true;
                _observer.RequestAction("pause", new List<object>());
                return;
            }
            
            if (keyStates[Keys.Escape] == KeyState.Up)
                _escToRelease = false;

            var speed = Utils.Range(GameSettings.SpeedScrollMax * _camera.Zoom, GameSettings.SpeedScrollMin, GameSettings.SpeedScrollMax);
            if (keyStates[Keys.W] == KeyState.Down) _camera.Move(new Vector2(0, -speed));
            if (keyStates[Keys.S] == KeyState.Down) _camera.Move(new Vector2(0, speed));
            if (keyStates[Keys.A] == KeyState.Down) _camera.Move(new Vector2(-speed, 0));
            if (keyStates[Keys.D] == KeyState.Down) _camera.Move(new Vector2(speed, 0));

            if (scroll > 0) _camera.Zoom = Utils.Range(_camera.Zoom*1.2f, _zoomMin, _zoomMax);
            else if (scroll < 0) _camera.Zoom = Utils.Range(_camera.Zoom/1.2f, _zoomMin, _zoomMax);
            

        }

        public void RegisterControls(IControlsObservable observable)
        {
            observable.Register(this,
                new List<Keys>{Keys.W, Keys.A, Keys.S, Keys.D, Keys.Escape},
                new List<MouseButtons>(), true);
        }
    }
}
