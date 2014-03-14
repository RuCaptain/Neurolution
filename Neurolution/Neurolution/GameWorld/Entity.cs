using System;
using System.Collections.Generic;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Neurolution.Graphics.Sprites;
using Neurolution.Helpers;

namespace Neurolution.GameWorld
{

    //Base game object class. 
    public abstract class Entity : IGameObject
    {
        //Body is a physical object, provided by FarseerPhysics library. It's used for collisions and ray tracing.
        public Body Body;
        public float Size;
        public Sprite CurrentSprite;
        public float Health = 0f;
        public float MaxHealth = 0f;
        protected float Speed = 0f;
        protected IActionObserver WorldManager;
        protected WorldProxy WorldProxy;

        protected Entity(WorldProxy proxy, Sprite sprite, Vector2 position, float rotation, float size, bool isRectangle = false)
        {
            Size = size;
            WorldProxy = proxy;
            CurrentSprite = sprite;

            //Creating body with specified shape (round or square)
            Body = isRectangle
                ? BodyFactory.CreateRectangle(WorldProxy.World, ConvertUnits.ToSimUnits(CurrentSprite.ObjectSize * size),
                    ConvertUnits.ToSimUnits(CurrentSprite.ObjectSize * size), 5f, position)
                : BodyFactory.CreateCircle(WorldProxy.World,
                    ConvertUnits.ToSimUnits(Math.Min(CurrentSprite.ObjectSize, CurrentSprite.ObjectSize) * size),
                    1.5f, position);

            Body.BodyType = BodyType.Dynamic;
            Body.Rotation = rotation;
            Body.Friction = 0.0f;
            Body.Restitution = 0.2f;
            Body.LinearDamping = 0.2f;
            Body.AngularDamping = 0.2f;
            Body.CollisionCategories = Category.All;
            Body.CollidesWith = Category.All;
            Body.LocalCenter = new Vector2(CurrentSprite.ObjectSize/2);
        }

        public string GetName()
        {
            return (string) Body.UserData;
        }

        public abstract void Update();
        public abstract void PerformAction(string action, List<object> parameters );
        public abstract Dictionary<string, object> GetCustomData();

        public float GetSpeed()
        {
            return Speed;
        }
    }
}
