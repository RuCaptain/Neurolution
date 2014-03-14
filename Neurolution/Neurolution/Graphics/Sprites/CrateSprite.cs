using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Neurolution.Graphics.Sprites
{
    public class CrateSprite : Sprite
    {
        public CrateSprite(IEnumerable<Texture2D> textures) : base(textures){}

        public override int AngleRange
        {
            get { return 90; }
        }

        public override float ObjectSize
        {
            get { return 200f; }
        }

        public override Color AverageColor
        {
            get
            {
                return new Color(180, 175, 165);
            }
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
