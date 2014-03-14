using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Neurolution.Graphics.Sprites
{
    public class CreatureAttackSprite : Sprite
    {
        public CreatureAttackSprite(IEnumerable<Texture2D> textures) : base(textures) {}

        protected override int AngleRange
        {
            get { return 90; }
        }

        protected override float ObjectSize
        {
            get { return 145f; }
        }

        public static new Color AverageColor()
        {
            return new Color(120, 120, 120);
        }

        public static String TextureName
        {
            get { return "SpriteCreature2"; }
        }

        public static int TexturesCount
        {
            get { return 12; }
        }
    }
}
