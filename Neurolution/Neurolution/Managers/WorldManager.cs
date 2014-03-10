using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Neurolution.Controls;
using Neurolution.Factories;
using Neurolution.GameWorld;
using Neurolution.Graphics;
using Neurolution.Helpers;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Neurolution.Managers
{
    //Main class. Processes the world and all it's entities.
    public class WorldManager : IManager
    {
        public World World;

        private int _creaturesSpawned;
        private int _initFoodSpawned;
        private int _cratesSpawned;
        private WorldPainter _worldPainter;
        public int MaxGeneration;
        public DateTime GameStartTime;
        private readonly List<Entity> _entitiesToDelete = new List<Entity>(); 
        public readonly List<Entity> EntityList = new List<Entity>();
        private EntityFactory _factory;

        public WorldManager()
        {
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);
            World = new World(Vector2.Zero);
            Settings.VelocityIterations = 1;
            Settings.PositionIterations = GameSettings.PositionIterations;
        }

        //Using for creating boundaries
        private static Vertices GetBounds()
        {
            var width = ConvertUnits.ToSimUnits(GameSettings.Borders.Width);
            var height = ConvertUnits.ToSimUnits(GameSettings.Borders.Height);
            var x = ConvertUnits.ToSimUnits(GameSettings.Borders.X);
            var y = ConvertUnits.ToSimUnits(GameSettings.Borders.Y);

            var bounds = new Vertices(4)
            {
                new Vector2(x, y),
                new Vector2(x + width, y),
                new Vector2(x + width, y + height),
                new Vector2(x, y + height)
            };

            return bounds;
        }

        public void Update(GameTime gameTime)
        {
            ProcessSpawns();

            foreach (var entity in EntityList)
            {
                entity.Update();

                if (!(entity.GetName() == "creature" && (int) entity.GetCustomData()["childrentospawn"] > 0)) continue;
                Spawn(_factory.CreateCreature(entity));
                entity.PerformAction("childspawned", new List<object>());
                break;
            }

            ProcessDestroys();

            World.Step(Math.Min(gameTime.ElapsedGameTime.Seconds, (1f / 60f)));

        }

        public void ProcessSpawns()
        {

            if (_initFoodSpawned < GameSettings.InitFood)
            {
                Spawn(_factory.CreateFood());
                _initFoodSpawned++;
                return;
            }

            if (_cratesSpawned < GameSettings.CrateCount)
            {
                Spawn(_factory.CreateCrate());
                _cratesSpawned++;
                return;
            }

            if (_creaturesSpawned == GameSettings.InitCreatures) return;
            Spawn(_factory.CreateCreature());
            _creaturesSpawned++;
        }

        private void ProcessDestroys()
        {

            foreach (var entity in _entitiesToDelete)
            {
                if (entity.GetName() == "creature") EntityList.AddRange(_factory.CreateLoot(entity));
                EntityList.Remove(entity);

            }
            _entitiesToDelete.Clear();
        }


        private void Spawn(Entity entity)
        {
            EntityList.Add(entity);
            if (entity.GetName() != "creature") return;

            var creatures = EntityList.FindAll(p => p.GetName() == "creature");
            MaxGeneration = creatures.Max(s => (int)s.GetCustomData()["generation"]);
        }
        public void Remove(Entity entity)
        {
            World.RemoveBody(entity.Body);
            _entitiesToDelete.Add(entity);
        }

        public void Draw(GameTime gameTime, bool doDrawInfo)
        {
            _worldPainter.DrawWorld(EntityList, MaxGeneration);
            _worldPainter.DrawHud(gameTime, EntityList, MaxGeneration, doDrawInfo, GameStartTime);
        }

        public void Initialize()
        {
            World.Clear();
            EntityList.Clear();
            _entitiesToDelete.Clear();
            _creaturesSpawned = _initFoodSpawned = 0;

            var bounds = GetBounds();

            var boundary = BodyFactory.CreateLoopShape(World, bounds);
            boundary.CollisionCategories = Category.All;
            boundary.CollidesWith = Category.All;
            boundary.BodyType = BodyType.Kinematic;

            GameStartTime = DateTime.Now;
        }

        public void LoadContent(SpriteFactory spriteFactory, WorldPainter painter)
        {
            _factory = new EntityFactory(spriteFactory, this);
            _worldPainter = painter;
        }

        public void Draw(GameTime gameTime)
        {
            Draw(gameTime, true);
        }

        public string GetName()
        {
            return "worldmanager";
        }

        public void Register(IActionObserver observer)
        {
        }

        public void ControlsUpdated(Dictionary<Keys, KeyState> keyStates, Dictionary<MouseButtons, ButtonState> buttonStates, Point cursorPoint, int scroll)
        {
        }

        public void RegisterControls(IControlsObservable observable)
        {
        }
    }
}
