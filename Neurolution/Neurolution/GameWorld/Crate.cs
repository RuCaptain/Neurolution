using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Neurolution.Graphics.Sprites;

namespace Neurolution.GameWorld
{
    public class Crate : Entity
    {
        //The simpliest object in game.
        //They do nothing, but they are visible and pushable.

        public Crate(WorldProxy proxy, Sprite sprite, Vector2 position, float rotation, float size)
            : base(proxy, sprite, position, rotation, size, true)
        {
            Body.UserData = "crate";
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
