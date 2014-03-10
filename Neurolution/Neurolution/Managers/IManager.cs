using Microsoft.Xna.Framework;
using Neurolution.Controls;
using Neurolution.Helpers;

namespace Neurolution.Managers
{
    //General manager interface
    public interface IManager : IControlsObserver, IActionObservable
    {
        void Initialize();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        string GetName();
    }
}
