using System.Collections.Generic;
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Neurolution.Controls
{
    //Provides an ability to register (and unregister accordingly) controls listeners.
    public interface IControlsObservable
    {
        void Register(IControlsObserver observer, List<Keys> keys, List<MouseButtons> buttons, bool alwaysNotify = false);
        void Unregister(IControlsObserver observer);
    }
}
