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

        public override int AngleRange
        {
            get { return 360; }
        }

        public override float ObjectSize
        {
            get { return 120f; }
        }

        public override Color AverageColor
        {
            get
            {
                return new Color(255, 0, 0);
            }
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
