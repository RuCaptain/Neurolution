using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Neurolution.Controls;
using Neurolution.GameWorld;
using Neurolution.Graphics;
using Neurolution.Graphics.Sprites;
using Neurolution.Helpers;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Neurolution.Managers
{

    //Used for drawing and other graphics-specified operations
    public class GraphicsManager : IManager
    {
        public SpriteBatch SpriteBatch;
        public readonly GraphicsDevice GraphicsDevice;
        public SpriteFont Font;
        public readonly Camera2D Camera = new Camera2D();
        private readonly Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>(); 
        public List<TileRow> Tiles = new List<TileRow>();
        public Tile GrayFloorTile;
        private IActionObserver _observer;
        private readonly Texture2D _pixel;



        #region Initization

        //====================================


        public GraphicsManager(GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, GameWindow window)
        {
            var graphics = graphicsDeviceManager;
            GraphicsDevice = graphicsDevice;
            graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            var form = (Form) Control.FromHandle(window.Handle);
            form.Resize += FormOnResize;
            form.MaximizeBox = true;
            Camera.Zoom = GameSettings.DefaultZoom;
            _pixel = CreatePixel(Color.White);
        }

        private void FormOnResize(object sender, EventArgs eventArgs)
        {
            _observer.RequestAction("resize", new List<object>());
        }

        //Loading textures and making tilemap
        public void LoadContent(ContentManager contentManager)
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            //Loading textures

            var path = GameSettings.ContentDirectory + "\\Graphics";
            var files = Utils.ListFilesRecursively(path);
            foreach (var file in files)
            {
                if (Path.GetExtension(GameSettings.ContentDirectory + file) != ".png") continue;

                var dir = new Uri(path).MakeRelativeUri(new Uri(file)).ToString();
                var name = Path.GetFileNameWithoutExtension(file);
                if (name == null) throw new ArgumentNullException();

                var relativeName = Path.GetDirectoryName(dir) + "\\" + name;
                var texture = contentManager.Load<Texture2D>(relativeName);
                _textures.Add(name, texture);
            }

            Font = contentManager.Load<SpriteFont>("HudFont");



            var tileTexture = GetTexture("TileFloorGray");
            var wallNW = GetTexture("TileWallBrownNW");
            var wallNE = GetTexture("TileWallBrownNE");
            GrayFloorTile = new Tile(0, 0, tileTexture);

            var width = GameSettings.WorldWidth;
            var height = GameSettings.WorldHeight;

            for (var x = -width/2; x < width/2; x++)
            {
                var row = new TileRow();
                for (var y = -height/2; y < height/2; y++)
                    row.Add(new Tile(x, y, tileTexture));
                Tiles.Add(row);
            }

            foreach (var tile in Tiles[0])
                tile.WallNW = wallNW;
            foreach (var row in Tiles)
                row.Last().WallNE = wallNE;
        }


        //====================================

        #endregion





        #region Functions

        //====================================


        public void Clear()
        {
            GraphicsDevice.Clear(Color.Black);
        }

        public Texture2D GetTexture(string name)
        {
            try
            {
                return _textures.First(p => p.Key == name).Value;
            }
            catch (Exception)
            {
                MessageBox.Show("Texture not found: " + name);
                throw;
            }
        }

        public Rectangle GetBounds()
        {
            return GraphicsDevice.Viewport.Bounds;
        }

        public Texture2D CreatePixel(Color color)
        {
            var texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new[] { color });
            return texture;
        }


        public void DrawSprite(Sprite sprite, Vector2 position, float rotation, float scale, float opacity = 1)
        {
            DrawSprite(sprite, position, rotation, scale, Color.White, opacity);
        }

        public void DrawSprite(Sprite sprite, Vector2 position, float rotation, float scale, Color color, float opacity = 1)
        {
            var transformedPos = Utils.TransformCoordinates(position);
            sprite.Update(position, rotation);
            SpriteBatch.Draw(sprite.Texture(), transformedPos, null, color * opacity, 0f, Utils.TextureOrigin(sprite), scale,
                SpriteEffects.None, Utils.RenderDepth(transformedPos, sprite.Texture().Height));
        }

        public void DrawTexture(Texture2D texture, Vector2 position, float layerDepth)
        {
            SpriteBatch.Draw(texture, position, null, Color.White, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, layerDepth);
        }

        public void DrawElement(Texture2D texture, Vector2 position)
        {
            DrawTexture(texture, position, 0f);
        }

        public void DrawRectangle(Rectangle rectangle, Texture2D texture, float scale = 1f)
        {
            SpriteBatch.Draw(texture, rectangle,
                new Rectangle(rectangle.X, rectangle.Y, (int)(rectangle.Width * scale), (int)(rectangle.Height * scale)),
                Color.White);
        }


        public void DrawBorder(Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
        {
            // Draw top line
            DrawColoredRectangle(new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);

            // Draw left line
            DrawColoredRectangle(new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

            // Draw right line
            DrawColoredRectangle(new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
                                            rectangleToDraw.Y,
                                            thicknessOfBorder,
                                            rectangleToDraw.Height), borderColor);
            // Draw bottom line
            DrawColoredRectangle(new Rectangle(rectangleToDraw.X,
                                            rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
                                            rectangleToDraw.Width,
                                            thicknessOfBorder), borderColor);
        }


        public void DrawColoredRectangle(Rectangle rectangle, Color color, float opacity = 1f)
        {
            SpriteBatch.Draw(_pixel, rectangle, color * opacity);
        }

        public void DrawBackground(Texture2D texture)
        {
            DrawRectangle(GetBounds(), texture);
        }
        
        //Capturing screenshot and writing it to file
        //Made by Brian Clifton (page not currently available)

        public void TakeScreenshot()
        {
            Clear();
            var w = GraphicsDevice.PresentationParameters.BackBufferWidth;
            var h = GraphicsDevice.PresentationParameters.BackBufferHeight;

            _observer.RequestAction("draw", new List<object>());

            var backBuffer = new int[w * h];
            GraphicsDevice.GetBackBufferData(backBuffer);

            //copy into a texture
            var texture = new Texture2D(GraphicsDevice, w, h, false, GraphicsDevice.PresentationParameters.BackBufferFormat);
            texture.SetData(backBuffer);

            SaveTexture(texture);
        }

        //Capturing screenshot and writing it to texture (used as GameMenu background)
        public Texture2D GameScreenshot(float overlayOpacity)
        {
            Clear();
            var w = GraphicsDevice.PresentationParameters.BackBufferWidth;
            var h = GraphicsDevice.PresentationParameters.BackBufferHeight;
            
            _observer.RequestAction("drawworld", new List<object>
            {false});

            SpriteBatch.Begin(SpriteSortMode.Immediate,
                BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);
            DrawColoredRectangle(GraphicsDevice.PresentationParameters.Bounds, Color.Black, 0.5f);
            SpriteBatch.End();

            var backBuffer = new int[w * h];

            GraphicsDevice.GetBackBufferData(backBuffer);

            //copy into a texture
            var texture = new Texture2D(GraphicsDevice, w, h, false, GraphicsDevice.PresentationParameters.BackBufferFormat);
            texture.SetData(backBuffer);

            return texture;
        }

        public void SaveTexture(Texture2D texture)
        {
            if (!Directory.Exists("Screenshots")) Directory.CreateDirectory("Screenshots");
            var stream =
                File.OpenWrite(GameSettings.GameDirectory + "\\Screenshots\\Screenshot " +
                               String.Format("{0:dd-MM-yyyy HH-mm-ss}", DateTime.Now) + ".png");
            texture.SaveAsPng(stream, texture.Width, texture.Height);
            stream.Close();
        }
        
        //====================================

        #endregion


        public void Initialize()
        {
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime)
        {
        }

        public void ControlsUpdated(Dictionary<Keys, KeyState> keyStates, Dictionary<MouseButtons, ButtonState> buttonStates, Point cursorPoint, int scroll)
        {
        }

        public void RegisterControls(IControlsObservable observable)
        {
        }

        public void Register(IActionObserver observer)
        {
            _observer = observer;
        }

        public string GetName()
        {
            return "graphicsmanager";
        }
    }
}
