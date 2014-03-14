using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework.Graphics;
using Neurolution.Graphics.Sprites;
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
            var textures = GetTextures(CreatureStandSprite.TextureName, CreatureStandSprite.TexturesCount);
            var sprite = new CreatureStandSprite(textures);

            return sprite;
        }

        public Sprite CreateCreatureAttackSprite()
        {
            var textures = GetTextures(CreatureAttackSprite.TextureName, CreatureAttackSprite.TexturesCount);
            var sprite = new CreatureAttackSprite(textures);

            return sprite;
        }

        public Sprite CreateFoodSprite()
        {
            var textures = GetTextures(FoodSprite.TextureName, FoodSprite.TexturesCount);
            var sprite = new FoodSprite(textures);

            return sprite;
        }

        public Sprite CreateCrateSprite()
        {
            var textures = GetTextures(CrateSprite.TextureName, CrateSprite.TexturesCount);
            var sprite = new CrateSprite(textures);

            return sprite;
        }
    }
}