using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Neurolution.Controls;
using Neurolution.Helpers;

namespace Neurolution.Menu
{
    public interface IMenu : IGameObject, IActionObservable, IControlsObserver
    {
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        void SetBackground(Texture2D texture);
        void Resize();
    }
}
