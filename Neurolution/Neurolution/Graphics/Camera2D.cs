using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Neurolution.Graphics
{
    //Camera is used for advanced displaying of world.
    //Made by David Amador: http://www.david-amador.com/2009/10/xna-camera-2d-with-zoom-and-rotation/

    public class Camera2D
    {
        private Matrix _transform; // Matrix Transform

        public Camera2D()
        {
            Zoom = 1.0f;
            Rotation = 0.0f;
            Pos = Vector2.Zero;
        }

        // Sets and gets zoom
        public float Zoom { get; set; }

        public float Rotation { get; set; }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            Pos += amount;
        }
        // Get set position
        public Vector2 Pos { get; set; }

        public Matrix GetTransformation(GraphicsDevice graphicsDevice)
        {
            _transform =       // Thanks to o KB o for this solution
                                         Matrix.CreateTranslation(new Vector3(-Pos.X, -Pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
            return _transform;
        }
 
    }
}
