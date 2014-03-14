using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Neurolution.Graphics.Sprites
{
    public class CrateSprite : Sprite
    {
        public CrateSprite(IEnumerable<Texture2D> textures) : base(textures){}

        protected override int AngleRange
        {
            get { return 90; }
        }

        protected override float ObjectSize
        {
            get { return 200f; }
        }

        public static new Color AverageColor()
        {
            return new Color(180, 175, 165);
        }

        public static String TextureName
        {
            get { return "SpriteCrate"; }
        }

        public static int TexturesCount
        {
            get { return 6; }
        }
    }
}
