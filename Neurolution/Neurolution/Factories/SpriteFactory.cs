using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework.Graphics;
using Neurolution.Graphics;
using Neurolution.Managers;

namespace Neurolution.Factories
{
    //Creates sprites for entities
    public class SpriteFactory
    {
        private readonly GraphicsManager _graphicsManager;
        public SpriteFactory(GraphicsManager manager)
        {
            _graphicsManager = manager;
        }

        private IEnumerable<Texture2D> GetTextures(string textureName, int count)
        {
            var textures = new List<Texture2D>();
            for (var i = 0; i < count; i++)
                textures.Add(_graphicsManager.GetTexture(textureName + (i + 1).ToString(CultureInfo.InvariantCulture)));
            return textures;
        }

        public Sprite CreateCreatureSprite()
        {
            var textures = GetTextures("SpriteCreature1", 12);
            var sprite = new Sprite(360, GameSettings.CreatureSpriteSize, textures);

            return sprite;
        }

        public Sprite CreateCreatureAttackSprite()
        {
            var textures = GetTextures("SpriteCreature2", 12);
            var sprite = new Sprite(360, GameSettings.CreatureSpriteSize, textures);

            return sprite;
        }

        public Sprite CreateFoodSprite()
        {
            var textures = GetTextures("SpriteMeat", 6);
            var sprite = new Sprite(360, GameSettings.FoodSpriteSize, textures);

            return sprite;
        }

        public Sprite CreateCrateSprite()
        {
            var textures = GetTextures("SpriteCrate", 6);
            var sprite = new Sprite(90, GameSettings.CrateSpriteSize, textures);

            return sprite;
        }
    }
}