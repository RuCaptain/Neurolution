using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Neurolution.Helpers;

namespace Neurolution.Graphics
{
    //Sprite is an graphical element of "dynamic" world object (which is not a part of world terrain).
    //It contains N textures for N angle intervals.
    //E.G., if it has 12 textures for 360°, the first texture will be displayed if object rotation is in range of [0, 30),
    //the second — for [30, 60), and so on.

    public class Sprite
    {
        public readonly Texture2D[] Textures;
        private readonly float _angleRange;
        private Texture2D _currentTexture;

        public Vector2 Position = new Vector2();
        public float Rotation;
        public float ObjectSize;


        public Sprite(int angleRange, float objectSize, IEnumerable<Texture2D> textures)
        {
            _angleRange = angleRange;
            Textures = textures.ToArray();
            ObjectSize = objectSize;
        }

        public void Update(Vector2 position, float rotation)
        {
            Position = position;
            Rotation = rotation + MathHelper.ToRadians(90);
            var angle = Utils.Normalize(MathHelper.ToDegrees(Rotation), _angleRange);
            var angleInterval = _angleRange/Textures.Length;
            _currentTexture = Textures[(int) Math.Ceiling(angle/angleInterval) - 1];
        }

        public Texture2D CurrentTexture()
        {
            return _currentTexture;
        }

    }
}
