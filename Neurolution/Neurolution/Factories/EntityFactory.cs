using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using Microsoft.Xna.Framework;
using Neurolution.GameWorld;
using Neurolution.Graphics;
using Neurolution.Helpers;
using Neurolution.Managers;
using RecurrentNetworkLibrary;

namespace Neurolution.Factories
{
    //Factory that creating game entities
    public class EntityFactory
    {
        private readonly SpriteFactory _spriteFactory;
        private readonly WorldProxy _worldProxy;

        public EntityFactory(SpriteFactory spriteFactory, WorldManager worldManager)
        {
            //Sprite factory is using for creating entities' own sprites
            _spriteFactory = spriteFactory;
            _worldProxy = new WorldProxy(worldManager);
        }

        private static Vector2 RandomPosition(float size, Sprite sprite)
        {

            var x = Utils.Random.Next(GameSettings.Borders.X + (int)(sprite.ObjectSize * size),
                    GameSettings.Borders.X + GameSettings.Borders.Width - (int)(sprite.ObjectSize * size));
            var y = Utils.Random.Next(GameSettings.Borders.Y + (int)(sprite.ObjectSize * size),
                    GameSettings.Borders.Y + GameSettings.Borders.Height - (int)(sprite.ObjectSize * size));
            return ConvertUnits.ToSimUnits(new Vector2(x, y));
        }

        private static float RandomRotation()
        {
            return MathHelper.ToRadians(Utils.Random.Next(360));
        }


        public Entity CreateCreature(float size)
        {
            var sprite = _spriteFactory.CreateCreatureSprite();
            var attackSprite = _spriteFactory.CreateCreatureAttackSprite();
            var position = RandomPosition(size, sprite);
            var rotation = MathHelper.ToRadians(Utils.Random.Next(0, 360));
            
            return new Creature(
                _worldProxy,
                sprite,
                attackSprite,
                position,
                rotation,
                size
                );
        }
        public Entity CreateCreature()
        {
            var size = Utils.RandomRange(GameSettings.CreatureSizeMin, GameSettings.CreatureSizeMax);
            return CreateCreature(size);
        }

        //Creating a child creature
        public Entity CreateCreature(Entity parent)
        {
            var generation = (int) parent.GetCustomData()["generation"];
            var sprite = _spriteFactory.CreateCreatureSprite();
            var attackSprite = _spriteFactory.CreateCreatureAttackSprite();

            //Position and size is relative to parent's ones
            var position = ConvertUnits.ToDisplayUnits(parent.Body.Position);
            var size =
                    Utils.Range(Utils.RandomSpread(parent.Size, 0.2f), GameSettings.CreatureSizeMin / 1.5f,
                        GameSettings.CreatureSizeMax * 1.5f);

            var x = Utils.Random.Next(Math.Max((int) (position.X - sprite.ObjectSize*size), GameSettings.Borders.X),
                Math.Min((int) (position.X + sprite.ObjectSize*size),
                    GameSettings.Borders.X + GameSettings.Borders.Width));
            var y = Utils.Random.Next(Math.Max((int) (position.Y - sprite.ObjectSize*size), GameSettings.Borders.Y),
                Math.Min((int) (position.Y + sprite.ObjectSize*size),
                    GameSettings.Borders.Y + GameSettings.Borders.Height));

            var creature = new Creature(_worldProxy, sprite, attackSprite, ConvertUnits.ToSimUnits(new Vector2(x, y)),
                RandomRotation(), size, generation + 1);


            //We need to clone parent's neural network to child (with some interferences)

            var parentNetwork = (RecurrentNetwork) parent.GetCustomData()["neuralnetwork"];
            var network = (RecurrentNetwork) creature.GetCustomData()["neuralnetwork"];

            for (var i = 0; i < network.Layers.Count(); i++)
            {
                var layer = parentNetwork.Layers.ToList();
                var newLayer = network.Layers.ToList();
                var neuronsCount = layer[i].NeuronCount;
                if (i == 0) neuronsCount -= layer[2].NeuronCount;
                for (var j = 0; j < neuronsCount; j++)
                {
                    var neurons = layer[i].Neurons.ToList();
                    var newNeurons = newLayer[i].Neurons.ToList();
                    for (var k = 0; k < neurons[j].SourceSynapses.Count; k++)
                        newNeurons[j].SourceSynapses[k].Weight = Utils.RandomSpread(
                            neurons[j].SourceSynapses[k].Weight, GameSettings.NetworkRandomSpread);
                    for (var k = 0; k < neurons[j].DestinationSynapses.Count; k++)
                        newNeurons[j].DestinationSynapses[k].Weight =
                            Utils.RandomSpread(neurons[j].DestinationSynapses[k].Weight,
                                GameSettings.NetworkRandomSpread);
                }
            }

            return creature;
        }

        public Entity CreateFood(float size)
        {
            var sprite = _spriteFactory.CreateFoodSprite();
            return new Food(
                _worldProxy,
                sprite,
                RandomPosition(size, sprite),
                RandomRotation(),
                size
                );
        }

        public Entity CreateFood()
        {
            return CreateFood(Utils.RandomSpread(GameSettings.FoodSize, GameSettings.FoodSizeSpread));
        }

        public Entity CreateCrate()
        {
            var sprite = _spriteFactory.CreateCrateSprite();
            var size = Utils.RandomRange(GameSettings.CrateSizeMin, GameSettings.CrateSizeMax);
            return new Crate(
                _worldProxy,
                sprite,
                RandomPosition(size, sprite),
                RandomRotation(),
                size
                );
        }

        //Creating a food around creature
        public List<Entity> CreateLoot(Entity creature)
        {
            Utils.StartTest();

            var sprite = _spriteFactory.CreateFoodSprite();
            var position = creature.Body.Position;
            var foodCount = (int)(GameSettings.CreatureLootCount * creature.Size);

            var foodList = new List<Entity>();
            for (var i = 0; i < foodCount; i++)
            {
                var x = NetworkUtils.RandomRange(Math.Max((int)ConvertUnits.ToDisplayUnits(position.X) - sprite.ObjectSize / 2, GameSettings.Borders.X + sprite.ObjectSize / 2),
                        Math.Min((int)ConvertUnits.ToDisplayUnits(position.X) + sprite.ObjectSize / 2, GameSettings.Borders.X + GameSettings.Borders.Width - sprite.ObjectSize / 2));
                var y = NetworkUtils.RandomRange(Math.Max((int)ConvertUnits.ToDisplayUnits(position.Y) - sprite.ObjectSize / 2, GameSettings.Borders.Y + sprite.ObjectSize / 2),
                        Math.Min((int)ConvertUnits.ToDisplayUnits(position.Y) + sprite.ObjectSize / 2, GameSettings.Borders.Y + GameSettings.Borders.Height - sprite.ObjectSize / 2));
                var newPosition = ConvertUnits.ToSimUnits(new Vector2(x, y));
                foodList.Add(new Food(
                    _worldProxy, 
                    sprite, 
                    newPosition, 
                    RandomRotation(), 
                    Utils.RandomSpread(creature.Size, GameSettings.FoodSizeSpread)));

            }

            return foodList;
            
        }
    }
}
