using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Neurolution.GameWorld
{
    public class Tile
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public Texture2D FloorTexture;
        public Texture2D WallNW;
        public Texture2D WallNE;

        public Tile(int x, int y, Texture2D floorTexture)
        {
            X = x;
            Y = y;
            FloorTexture = floorTexture;
            Width = floorTexture.Width - 4; //Needed to reduce seams
            Height = FloorTexture.Height - 4;
        }
    }

    public class TileRow : List<Tile>
    {
    }
}
