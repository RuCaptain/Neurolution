using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Neurolution.Graphics.Sprites;

namespace Neurolution.GameWorld
{
    public class Food : Entity
    {

        public Food(WorldProxy proxy, Sprite sprite, Vector2 position, float rotation, float size = 1f)
            : base(proxy, sprite, position, rotation, size)
        {
            Body.UserData = "food";
        }

        public override void Update()
        {
        }

        public override void PerformAction(string action, List<object> parameters)
        {
        }

        public override Dictionary<string, object> GetCustomData()
        {
            return new Dictionary<string, object>();
        }

    }
}
