using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Neurolution.Controls;
using Neurolution.Helpers;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Neurolution.Screens
{
    public class MenuScreen : IScreen
    {
        private IActionObserver _observer;

        public string GetName()
        {
            return "menuscreen";
        }

        public void PerformAction(string action, List<object> parameters)
        {
        }

        public Dictionary<string, object> GetCustomData()
        {
            return new Dictionary<string, object>();
        }

        public void Initialize()
        {
        }

        public void Update(GameTime gameTime)
        {
            //Request updating menu manager
            _observer.RequestAction("update", new List<object>{
                "menumanager"});
        }

        public void Draw(GameTime gameTime)
        {
            _observer.RequestAction("draw", new List<object>
            {
                "menumanager"
            });
        }

        public void Register(IActionObserver observer)
        {
            _observer = observer;
        }

        public void ControlsUpdated(Dictionary<Keys, KeyState> keyStates, Dictionary<MouseButtons, ButtonState> buttonStates, Point cursorPoint, int scroll)
        {
        }

        public void RegisterControls(IControlsObservable observable)
        {
        }
    }
}
