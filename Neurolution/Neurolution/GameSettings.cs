using System.IO;
using Microsoft.Xna.Framework;
using Neurolution.Helpers;

namespace Neurolution
{

    //Global settings class.
    //Containts configuration data and hard-coded constants.
    public static class GameSettings
    {
        public static int WorldWidth;
        public static int WorldHeight;

        public static Rectangle Borders;

        public const int TileHeight = 172;
        public const int PositionIterations = 50;
        public const float DefaultZoom = 0.2f;
        public const bool DisplayCursor = true;

        public static readonly string GameDirectory =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static readonly string ContentDirectory = GameDirectory + "\\Content";

        public static readonly string ConfigurationFile = GameDirectory + "\\" +
                                                          Path.GetFileNameWithoutExtension(
                                                              System.Reflection.Assembly.GetExecutingAssembly().Location) +
                                                          ".cfg";

        public static int InitCreatures;
        public static int InitFood;
        public static int CrateCount;
        public static float CrateSizeMin;
        public static float CrateSizeMax;

        public static float CreatureSizeMin;
        public static float CreatureSizeMax;
        public static int CreatureChildrenMin;
        public static int CreatureChildrenMax;
        public static int CreatureLootCount;
        public static float CreatureMovingSpeed;
        public static float CreatureRotatingSpeed;
        public static float CreatureBreedingEnergy;
        public static float CreatureBreedingTimer;
        public static float CreatureAttackDamage;
        public static float CreatureHungerDamage;
        public static int CreatureHungerDamageInterval;
        public static float CreatureLookRange;
        public static float CreatureSniffRange;
        public static float FoodSatiety;

        public const int CreatureMaxIdle = 5;
        public const int CreatureDamageDuration = 20;

        public static float NetworkInitMinValue;
        public static float NetworkInitMaxValue;
        public static float NetworkLearningRate;
        public static float NetworkRandomSpread;
        public static float NetworkSensorsAmplifier;
        public static float NetworkThreshold;

        public const int NetworkInputs = 48;
        public const int NetworkOutputs = 8;
        public const int NetworkHiddenLayers = 1;

        public static readonly float[] LossOfEnergy =
        {
            0.004f, //stand
            0.014f,  //move
            0.008f, //attack
            0.028f   //run
        };

        public static readonly float[] Rehabilitation =
        {
            0.07f,  //stand
            0.013f, //move
            0.009f, //attack
            0.005f  //run
        };

        public const int SpeedScrollMax = 50;
        public const int SpeedScrollMin = 20;
        public const int HealthBarWidth = 100;
        public const int HealthBarHeight = 20;

        public const float ClawsSize = 0.6f;
        public const float FoodSize = 1.2f;
        public const float FoodSizeSpread = 0.3f;

        //Loading configuration from file
        public static void Load(Configuration config)
        {
            WorldWidth = config.GetInt("World", "WorldWidth", 96);
            WorldHeight = config.GetInt("World", "WorldHeight", 96);
            Borders = new Rectangle(-WorldWidth * TileHeight / 2, -WorldHeight * TileHeight / 2,
            WorldWidth * TileHeight, WorldHeight * TileHeight);

            InitCreatures = config.GetInt("World", "InitCreatures", 25);
            InitFood = config.GetInt("World", "InitFood", 300);
            CrateCount = config.GetInt("World", "CrateCount", 20);
            CrateSizeMin = config.GetInt("World", "CrateSizeMin", 10)/10f;
            CrateSizeMax = config.GetInt("World", "CrateSizeMax", 30)/10f;

            CreatureSizeMin = config.GetInt("Creature", "SizeMin", 10)/10f;
            CreatureSizeMax = config.GetInt("Creature", "SizeMax", 20)/10f;
            CreatureChildrenMin = config.GetInt("Creature", "ChildrenMin", 1);
            CreatureChildrenMax = config.GetInt("Creature", "ChildrenMax", 4);
            CreatureLootCount = config.GetInt("Creature", "LootCount", 4);
            CreatureMovingSpeed = config.GetFloat("Creature", "MovingSpeed", 8f);
            CreatureRotatingSpeed = config.GetFloat("Creature", "RotatingSpeed", 2f);
            CreatureBreedingEnergy = config.GetFloat("Creature", "BreedingEnergy", 100f);
            CreatureBreedingTimer = config.GetFloat("Creature", "BreedingTimer", 0.01f);
            CreatureAttackDamage = config.GetFloat("Creature", "AttackDamage", 25f);
            CreatureHungerDamage = config.GetFloat("Creature", "HungerDamage", 5f);
            CreatureHungerDamageInterval = config.GetInt("Creature", "HungerDamageInterval", 80);
            CreatureLookRange = config.GetFloat("Creature", "LookRange", 8f);
            CreatureSniffRange = config.GetFloat("Creature", "SniffRange", 4f);
            FoodSatiety = config.GetFloat("Creature", "FoodSatiety", 12.5f);

            NetworkInitMinValue = config.GetFloat("Network", "InitMinValue", 0.01f);
            NetworkInitMaxValue = config.GetFloat("Network", "InitMaxValue", 0.105f);
            NetworkLearningRate = config.GetFloat("Network", "LearningRate", 0.15f);
            NetworkRandomSpread = config.GetFloat("Network", "RandomSpread", 0.1f);
            NetworkSensorsAmplifier = config.GetFloat("Network", "SensorsAmplifier", 1.2f);
            NetworkThreshold = config.GetFloat("Network", "Threshold", 2.2f);

        }
    }
}
