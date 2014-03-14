using Microsoft.Xna.Framework;

namespace Neurolution.GameWorld
{
    //Information about certain object. Provided by WorldProxy.
    public class EntityInfo
    {
        public float Rotation = 0f;
        public Vector2 Position = Vector2.Zero;
        public float Size = 0f;
        public string Name = "";
        public float Speed = 0f;
        public Color AverageColor = Color.Black;
    }
}
