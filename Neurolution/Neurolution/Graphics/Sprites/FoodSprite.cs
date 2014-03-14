using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Neurolution.Graphics.Sprites
{
    public class FoodSprite : Sprite
    {
        public FoodSprite(IEnumerable<Texture2D> textures) : base (textures)
        {
        }

        protected override int AngleRange
        {
            get { return 360; }
        }

        protected override float ObjectSize
        {
            get { return 140f; }
        }

        public static new Color AverageColor()
        {
            return new Color(255, 0, 0);
        }

        public static String TextureName
        {
            get { return "SpriteMeat"; }
        }

        public static int TexturesCount
        {
            get { return 6; }
        }
    }
}
