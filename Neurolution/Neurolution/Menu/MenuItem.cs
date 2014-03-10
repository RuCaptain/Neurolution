using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Neurolution.Menu
{
    public abstract class MenuItem : IGameObject
    {
        public Vector2 Position;
        protected Texture2D Texture;
        public abstract event EventHandler Click;
        public abstract void DoClick();

        protected MenuItem(Texture2D initTexture)
        {
            Texture = initTexture;
        }

        public Texture2D CurrentTexture()
        {
            return Texture;
        }

        public Rectangle GetBounds()
        {
            return new Rectangle(
                (int)Position.X - Texture.Width / 2,
                (int)Position.Y - Texture.Height / 2,
                Texture.Width,
                Texture.Height
                );
        }

        public abstract string GetName();

        public abstract void PerformAction(string action, List<object> parameters);

        public abstract Dictionary<string, object> GetCustomData();
    }
}
