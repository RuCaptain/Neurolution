using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Neurolution.Helpers;
using Neurolution.Managers;

namespace Neurolution.GameWorld
{
    //WorldProxy is an interface of restricted access, allows creatures to gather information of outer world.
    public class WorldProxy
    {
        private readonly WorldManager _worldManager;
        public readonly World World;
        public WorldProxy(WorldManager manager)
        {
            _worldManager = manager;
            World = _worldManager.World;
        }

        public bool EntityExists(Body body)
        {
            return _worldManager.EntityList.Exists(p => p.Body == body);
        }

        public EntityInfo GetEntityInfo(Entity target)
        {
            return new EntityInfo
            {
                Name = target.GetName(),
                Position = target.Body.Position,
                Rotation = target.Body.Rotation,
                Size = target.Size,
                Speed = target.GetSpeed(),
                AverageColor = target.CurrentSprite.AverageColor
            };
        }
        public EntityInfo GetEntityInfo(Body targetBody)
        {
            var target = _worldManager.EntityList.Find(p => p.Body == targetBody);
            return target == null ? null : GetEntityInfo(target);
        }

        public void DamageEntity(Body targetBody, float damage)
        {
            var target = _worldManager.EntityList.Find(p => p.Body == targetBody);
            if (target == null) return;
            target.PerformAction("damage", new List<object>
            {
                damage
            });
        }

        public void RemoveEntity(Body targetBody)
        {
            var target = _worldManager.EntityList.Find(p => p.Body == targetBody);
            if (target == null) return;
            _worldManager.Remove(target);
        }

        public void Kill(Entity sender)
        {
            _worldManager.Remove(sender);
        }

        public List<EntityInfo> NearestEntities(Entity sender, float distance)
        {
            var entities = (from entity in _worldManager.EntityList
                let d = Utils.GetDistance((Vector2) sender.GetCustomData()["faceposition"], sender.Body.Position)
                where d <= distance
                select entity).ToList();

            return entities.Select(GetEntityInfo).ToList();
        }
    }
}
