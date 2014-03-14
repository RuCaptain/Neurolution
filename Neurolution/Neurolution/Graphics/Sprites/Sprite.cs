using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Neurolution.Helpers;

namespace Neurolution.Graphics.Sprites
{
    //Sprite is an graphical element of "dynamic" world object (which is not a part of world terrain).
    //It contains N textures for N angle intervals.
    //E.G., if it has 12 textures for 360°, the first texture will be displayed if object rotation is in range of [0, 30),
    //the second — for [30, 60), and so on.

    public abstract class Sprite
    {
        protected readonly Texture2D[] Textures;
        public abstract int AngleRange { get; }
        public abstract float ObjectSize { get; }
        public abstract Color AverageColor { get; }
        protected Texture2D CurrentTexture;

        public Vector2 Position = new Vector2();
        public float Rotation;


        protected Sprite(IEnumerable<Texture2D> textures)
        {
            Textures = textures.ToArray();
        }

        public virtual void Update(Vector2 position, float rotation)
        {
            Position = position;
            Rotation = rotation + MathHelper.ToRadians(90);
            var angle = Utils.Normalize(MathHelper.ToDegrees(Rotation), AngleRange);
            var angleInterval = AngleRange/Textures.Length;
            CurrentTexture = Textures[(int) Math.Ceiling(angle/angleInterval) - 1];
        }

        public Texture2D Texture()
        {
            return CurrentTexture;
        }

    }
}
