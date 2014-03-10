using Microsoft.Xna.Framework;
using Neurolution.Controls;
using Neurolution.Helpers;

namespace Neurolution.Screens
{
    //Screen is an sort of united Presentation and Controller parts in MVC.
    public interface IScreen : IGameObject, IActionObservable, IControlsObserver
    {
        void Initialize();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
