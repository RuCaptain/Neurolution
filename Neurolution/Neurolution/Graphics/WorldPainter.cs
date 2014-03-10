using System;
using System.Collections.Generic;
using FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Neurolution.GameWorld;
using Neurolution.Helpers;
using Neurolution.Managers;

namespace Neurolution.Graphics
{
    //Draws the world (tiles, entities, HUD etc).
    public class WorldPainter : Painter
    {
        private readonly List<TileRow> _tiles;

        public WorldPainter(GraphicsManager manager) : base(manager)
        {
            _tiles = manager.Tiles;
        }


        public void StartDrawingWorld()
        {
            GraphicsManager.SpriteBatch.Begin(SpriteSortMode.BackToFront,
                BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone,
                null, GraphicsManager.Camera.GetTransformation(GraphicsManager.GraphicsDevice));
        }

        public void StartDrawingOverlays()
        {
            GraphicsManager.SpriteBatch.Begin(SpriteSortMode.BackToFront,
                BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null,
                GraphicsManager.Camera.GetTransformation(GraphicsManager.GraphicsDevice));
        }

        public void StartDrawingHud()
        {
            GraphicsManager.SpriteBatch.Begin(SpriteSortMode.Immediate,
                BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone,
                null, GraphicsManager.Camera.GetTransformation(GraphicsManager.GraphicsDevice));
        }

        public void DrawWorld(List<Entity> entities, int maxGeneration)
        {

            StartDrawingWorld();

            //Drawing tiles and sprites
            DrawTiles();
            foreach (var entity in entities)
                GraphicsManager.DrawSprite(entity.CurrentSprite, ConvertUnits.ToDisplayUnits(entity.Body.Position), entity.Body.Rotation, entity.Size);

            var creatures = entities.FindAll(p => p.GetName() == "creature");
            if (creatures.Count == 0) //If there's no one creature, stop drawing.
            {
                EndDrawing();
                return;
            }
            float opacity;

            //Except of younger creatures, which displays normally, older ones will be more transparent and "whiter".
            foreach (var creature in creatures)
            {
                var generation = maxGeneration - (int)creature.GetCustomData()["generation"];

                //!! Transparenty not working for some reason
                //TODO: Fix transparenty

                switch (generation)
                {
                    case 0:
                        opacity = 1f;
                        break;
                    case 1:
                        opacity = 0.7f;
                        break;
                    case 2:
                        opacity = 0.5f;
                        break;
                    default:
                        opacity = 0.4f;
                        break;
                }

                //If creature damaged by other one, it will be tinted red. 
                if ((int)creature.GetCustomData()["damagetimer"] > 0)
                    GraphicsManager.DrawSprite(creature.CurrentSprite,
                        ConvertUnits.ToDisplayUnits(creature.Body.Position),
                        creature.Body.Rotation, creature.Size, Color.Red, opacity);
                else
                    GraphicsManager.DrawSprite(creature.CurrentSprite,
                        ConvertUnits.ToDisplayUnits(creature.Body.Position),
                        creature.Body.Rotation, creature.Size, opacity);
            }

            EndDrawing();

            StartDrawingOverlays();

            foreach (var creature in creatures)
            {
                var generation = maxGeneration - (int)creature.GetCustomData()["generation"];
                if(generation == 0) continue;
                switch (generation)
                {
                    case 1:
                        opacity = 0.6f;
                        break;
                    case 2:
                        opacity = 0.7f;
                        break;
                    default:
                        opacity = 0.8f;
                        break;
                }
                //Making older creatures to look whiter
                GraphicsManager.DrawSprite(creature.CurrentSprite,
                    ConvertUnits.ToDisplayUnits(creature.Body.Position),
                    creature.Body.Rotation, creature.Size, Color.White, opacity);


            }



            EndDrawing();
        }


        //Drawing creature-specified information bars (health and "breeding energy"), and general game information text

        public void DrawHud(GameTime gameTime, List<Entity> entities, int maxGeneration, bool doDrawInfo, DateTime gameStartTime)
        {
            StartDrawingHud();

            var creatures = entities.FindAll(p => p.GetName() == "creature");
            foreach (var creature in creatures)
            {
                if ((float)creature.GetCustomData()["health"] < (float)creature.GetCustomData()["maxhealth"]) DrawHealthBar(creature);
                var energy = (float)creature.GetCustomData()["breedingsatiety"] + (float)creature.GetCustomData()["breedingtimer"];
                if (energy > 0) DrawBreedingEnergyBar(creature);

            }

            EndDrawing();

            if (!doDrawInfo) return;

            StartDrawing();

            var time = DateTime.Now - gameStartTime;

            GraphicsManager.DrawColoredRectangle(new Rectangle(0, 0, GraphicsManager.GraphicsDevice.Viewport.Width, 20), Color.White);
            GraphicsManager.SpriteBatch.DrawString(GraphicsManager.Font,
                "Creatures: " + creatures.Count + ", Current generation:" +
                (maxGeneration + 1) + ", Game time: " +
                string.Format("{0:hh\\:mm}", time)
                 + ", " + (int)(1 / gameTime.ElapsedGameTime.TotalSeconds) + " FPS",
                new Vector2(20, 5), Color.Black);


            EndDrawing();

        }

        //Drawing tiles (floors and walls)

        public void DrawTiles()
        {
            foreach (var row in _tiles)
                foreach (var tile in row)
                {
                    var coords = Utils.TileCoordToRealCoord(tile);
                    GraphicsManager.DrawTexture(
                        tile.FloorTexture,
                        coords,
                        1f
                        );
                    if (tile.WallNW != null)
                        DrawWallNW(tile);
                    if (tile.WallNE != null)
                        DrawWallNE(tile);
                }
        }

        public void DrawWallNW(Tile tile)
        {
            var coords = Utils.TileCoordToRealCoord(tile);
            coords.X -= (float)tile.WallNW.Width / 4;
            coords.Y -= (float)tile.WallNW.Height / 2;

            GraphicsManager.DrawTexture(
                tile.WallNW,
                coords,
                Utils.RenderDepth(coords, tile.WallNW.Height)
                );
        }

        public void DrawWallNE(Tile tile)
        {
            var coords = Utils.TileCoordToRealCoord(tile);
            coords.X += (float)tile.WallNE.Width / 4;
            coords.Y -= (float)tile.WallNE.Height / 2;

            GraphicsManager.DrawTexture(
                tile.WallNE,
                coords,
                Utils.RenderDepth(coords, tile.WallNE.Height)
                );
        }


        private void DrawHealthBar(Entity creature)
        {
            var health = (float) creature.GetCustomData()["health"];
            var maxHealth = (float)creature.GetCustomData()["maxhealth"];

            var position =
                Utils.TransformCoordinates(new Vector2(ConvertUnits.ToDisplayUnits(creature.Body.Position.X),
                    ConvertUnits.ToDisplayUnits(creature.Body.Position.Y)));
            var centerX = position.X;
            var centerY = position.Y - creature.CurrentSprite.ObjectSize * 2 * creature.Size - 40 +
                          GameSettings.HealthBarHeight - 20 / creature.Size;

            var rectangle = new Rectangle(
                    (int)(centerX - (float)GameSettings.HealthBarWidth / 2),
                    (int)(centerY - (float)GameSettings.HealthBarHeight / 2),
                    (int)Math.Ceiling((health / maxHealth) * GameSettings.HealthBarWidth),
                    GameSettings.HealthBarHeight
                    );
            var borderRectangle = new Rectangle(
                    (int)(centerX - (float)GameSettings.HealthBarWidth / 2 - 2),
                    (int)(centerY - (float)GameSettings.HealthBarHeight / 2 - 2),
                    GameSettings.HealthBarWidth + 4,
                    GameSettings.HealthBarHeight + 4

                );

            var color = new Color(
                    (int)((maxHealth - health) * 2.55f),
                    (int)(health / maxHealth * 255),
                    0
                    );

            GraphicsManager.DrawBorder(borderRectangle, 2, Color.Black);
            GraphicsManager.DrawColoredRectangle(rectangle, color);
        }


        private void DrawBreedingEnergyBar(Entity creature)
        {
            var breedingSatiety = (float) creature.GetCustomData()["breedingsatiety"];
            var breedingTimer = (float) creature.GetCustomData()["breedingtimer"];
            var breedingEnergy = (float)creature.GetCustomData()["breedingenergy"];
            var energy = breedingSatiety + breedingTimer;


            var position =
                Utils.TransformCoordinates(new Vector2(ConvertUnits.ToDisplayUnits(creature.Body.Position.X),
                    ConvertUnits.ToDisplayUnits(creature.Body.Position.Y)));
            var centerX = position.X;
            var centerY = position.Y - creature.CurrentSprite.ObjectSize * 2 * creature.Size - 80 +
                          GameSettings.HealthBarHeight - 20 / creature.Size;

            
            var rectangle = new Rectangle(
                (int)(centerX - (float)GameSettings.HealthBarWidth / 2),
                (int)(centerY - (float)GameSettings.HealthBarHeight / 2),
                (int)Math.Ceiling((energy / breedingEnergy) * GameSettings.HealthBarWidth),
                GameSettings.HealthBarHeight
                );

            var borderRectangle = new Rectangle(
                    (int)(centerX - (float)GameSettings.HealthBarWidth / 2 - 2),
                    (int)(centerY - (float)GameSettings.HealthBarHeight / 2 - 2),
                    GameSettings.HealthBarWidth + 4,
                    GameSettings.HealthBarHeight + 4

                );

            var color = new Color(
                255,
                (int)(energy / breedingEnergy * 255),
                (int)((breedingEnergy - energy) * 2.55f)
                );

            GraphicsManager.DrawBorder(borderRectangle, 2, Color.Black);
            GraphicsManager.DrawColoredRectangle(rectangle, color);
        }
    }
}
