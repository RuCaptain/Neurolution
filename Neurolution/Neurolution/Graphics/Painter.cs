using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Neurolution.Managers;

namespace Neurolution.Graphics
{
    //Used for drawing. Provied access to GraphicsManager, but allowed for subclasses only.
    public abstract class Painter
    {
        protected readonly GraphicsManager GraphicsManager;

        protected Painter(GraphicsManager manager)
        {
            GraphicsManager = manager;
        }

        public void StartDrawing()
        {
            GraphicsManager.SpriteBatch.Begin(SpriteSortMode.Immediate,
                BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);
        }

        public void EndDrawing()
        {
            GraphicsManager.SpriteBatch.End();
        }

        public Rectangle GetBounds()
        {
            return GraphicsManager.GetBounds();
        }

        public Texture2D CreatePixel(Color color)
        {
            return GraphicsManager.CreatePixel(color);
        }
    }
}
