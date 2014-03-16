using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Neurolution.Graphics.Sprites;
using Neurolution.Helpers;
using RecurrentNetworkLibrary;

namespace Neurolution.GameWorld
{
    //Creature class. Contains various parameters, some features, genome (ANN template) and active ANN.

    //Creature reproduces when it collected a necessary amount of energy.
    //Basically, energy collectes when creature is fed. BreedingTimer is slowly increasing.
    //But when creature eats more than necessary, it gives an extra energy (BreedingSatiety).
    //So, total energy is (BreedingTimer + BreedingSatiety).
    //When energy reaches an certain threshold the creature reproduces, and energy resets.

    public class Creature : Entity
    {
        #region Definitions

        //=======================================

        //Claws is using to eat and to damage other creatures
        private readonly Fixture _claws;

        //List of action states (to do or not to do)
        //TODO: Replace with Action abstract class (and it's subclasses of course)

        private readonly Dictionary<CreatureActions, bool> _actions = new Dictionary<CreatureActions, bool>();
        private readonly Sprite _attackSprite;
        private readonly Sprite _normalSprite;

        private float _speed;
        private float _acceleration;
        private readonly int _generation;

        private readonly RecurrentNetwork _neuralNetwork;
        private readonly float _networkThreshold;

        //Life parameters
        private float _satiety = 100f;
        private readonly float _breedingEnergy;
        private float _breedingSatiety; 
        private float _breedingTimer;
        private readonly float _maxSpeed;
        private readonly float _maxRotatingSpeed;
        private int _childrenToSpawn;

        private int _idleTimer;
        private float _maxDamage = GameSettings.CreatureAttackDamage;
        private float _maxSatiety = GameSettings.FoodSatiety;
        private float _attackedDamage;
        private int _damageTimer;
        private int _hungerDamageTimer;
        
        //Data of last watching
        private int _currentLookingDirection;
        private List<float> _lookDistance;
        private List<float> _lookSize;
        private List<float> _lookSpeed;
        private List<float> _lookSpace;
        private List<float> _lookRotation;
        private List<Color> _lookColor;
        private Vector2 _lookDirection;
        private float _smellFoodDistance;
        private float _smellCreatureDistance;
        private Vector2 _facePosition;

        //Using for comparing the own and target's parameters
        private float _maxSize = 1f;
        private float _maxTargetSpeed = GameSettings.CreatureMovingSpeed;

        private readonly float _maxSmellDistance;


        //=======================================

        #endregion

        #region Base functions

        public Creature(WorldProxy proxy, Sprite sprite, Sprite attackSprite, Vector2 position, float rotation, float size, int generation = 0) 
            : base (proxy, sprite, position, rotation, size)
        {
            Body.UserData = "creature";
            _generation = generation;
            _normalSprite = sprite;
            _attackSprite = attackSprite;

            //Most of parameters are based on creature size
            Health = MaxHealth = size*100f;
            _maxSpeed = GameSettings.CreatureMovingSpeed/size;
            _maxRotatingSpeed = GameSettings.CreatureRotatingSpeed/size;
            _breedingEnergy = GameSettings.CreatureBreedingEnergy*size;
            _maxSmellDistance = ConvertUnits.ToSimUnits(CurrentSprite.ObjectSize*
                                                         size*GameSettings.CreatureSniffRange);

            //Initizating the network layer
            _neuralNetwork = new RecurrentNetwork(GameSettings.NetworkInputs, GameSettings.NetworkOutputs,
                GameSettings.NetworkHiddenLayers, GameSettings.NetworkHiddenNeurons);
            _neuralNetwork.Layers[1 + GameSettings.NetworkHiddenLayers].RecurrentLayer = null;
            _neuralNetwork.Layers[2 + GameSettings.NetworkHiddenLayers].OutputLayer = _neuralNetwork.Layers[GameSettings.NetworkHiddenLayers];
            _neuralNetwork.Layers[GameSettings.NetworkHiddenLayers].RecurrentLayer = _neuralNetwork.Layers[2 + GameSettings.NetworkHiddenLayers];
            _neuralNetwork.Initialize(GameSettings.NetworkInitMinValue, GameSettings.NetworkInitMaxValue);
            _neuralNetwork.LearningRate = NetworkUtils.RandomSpread(GameSettings.NetworkLearningRate, GameSettings.NetworkRandomSpread);
            _networkThreshold = Utils.RandomSpread(GameSettings.NetworkThreshold, GameSettings.NetworkRandomSpread);

            Body.Rotation = 0;

            //Creating a claws
            _claws = FixtureFactory.AttachCircle(
                ConvertUnits.ToSimUnits(CurrentSprite.ObjectSize*
                                        size*GameSettings.ClawsSize), 1f, Body,
                ConvertUnits.ToSimUnits(new Vector2(CurrentSprite.ObjectSize * size * (1 - GameSettings.ClawsSize/4), 0)), "creature");
            _claws.CollidesWith = Category.All;
            _claws.OnCollision = BodyOnOnCollision;
            Body.Rotation = rotation;

            for(var i=0; i < GameSettings.NetworkOutputs; i++)
                _actions.Add((CreatureActions)i, false);

        }

        //Processing collision between own claws and collided entity
        private bool BodyOnOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if ((string) fixtureB.Body.UserData == "creature" && _actions[CreatureActions.Attack])
            {
                if (!WorldProxy.EntityExists(fixtureB.Body))
                    return true;

                WorldProxy.DamageEntity(fixtureB.Body, GameSettings.CreatureAttackDamage*Size);
                Push(-GameSettings.CreatureMovingSpeed * 2 / Size);
            }
            else if ((string) fixtureB.Body.UserData == "food")
            {
                var food = WorldProxy.GetEntityInfo(fixtureB.Body);
                ProcessEating(food.Size);
                WorldProxy.RemoveEntity(fixtureB.Body);

                return false;
            }
            
            return true;
        }

        public override void Update()
        {

            ProcessLifeParameters();

            if (_idleTimer == GameSettings.CreatureMaxIdle)
            {
                //Computing the claws position
                Transform transform;
                Body.GetTransform(out transform);
                AABB aabb;
                _claws.Shape.ComputeAABB(out aabb, ref transform, 0);
                _facePosition = aabb.Center;

                //Gathering information from outside
                Look();
                Sniff();

                //Asking network what to do
                ProcessNetwork();

                _idleTimer = 0;
            }
            _idleTimer++;

            if (Math.Abs(_acceleration) >= 0.4f)
            {
                Move(_acceleration);
                _acceleration *= 0.95f;
            }
            else _acceleration = 0;

            ProcessActions();
        }


        private void ProcessLifeParameters()
        {
            //Updating satiety. Enegry loss dependences on what creature is doing now.
            if (_satiety > 0)
            {
                if (_actions[CreatureActions.MoveForward])
                {
                    if (_actions[CreatureActions.Run])
                        _satiety -= GameSettings.LossOfEnergy[3];
                    else
                        _satiety -= GameSettings.LossOfEnergy[1];
                }
                if (_actions[CreatureActions.Attack])
                    _satiety -= GameSettings.LossOfEnergy[2];
                else if (_actions[CreatureActions.RotateLeft] || _actions[CreatureActions.RotateRight])
                    _satiety -= GameSettings.LossOfEnergy[1];
                else _satiety -= GameSettings.LossOfEnergy[0];
            }

            //If creature extremely hungry, it loses HP

            if (_satiety <= 5)
                if (_hungerDamageTimer == GameSettings.CreatureHungerDamageInterval)
                {
                    Damage(GameSettings.CreatureHungerDamage);
                    _hungerDamageTimer = 0;
                }
                else _hungerDamageTimer++;
            else if (Health < MaxHealth && _satiety >= 50)
            {
                //But if it fed, HP regenerates
                if (_actions[CreatureActions.MoveForward])
                {
                    if (_actions[CreatureActions.Run])
                        Heal(GameSettings.Rehabilitation[3]);
                    else if (_actions[CreatureActions.Attack])
                        Heal(GameSettings.Rehabilitation[2]);
                    else
                        Heal(GameSettings.Rehabilitation[1]);
                }
                else if (_actions[CreatureActions.Attack])
                    Heal(GameSettings.Rehabilitation[2]);
                else if (_actions[CreatureActions.RotateLeft] || _actions[CreatureActions.RotateRight])
                    Heal(GameSettings.Rehabilitation[1]);
                else Heal(GameSettings.Rehabilitation[0]);
            }

            if (_damageTimer > 0)
                _damageTimer--;

            if (!(_breedingTimer < _breedingEnergy) || !(_satiety >= 75f)) return;
            _breedingTimer += GameSettings.CreatureBreedingTimer;
            ProcessBreedingEnergy();
        }


        private void ProcessActions()
        {

            if (_actions[CreatureActions.MoveForward])
            {
                Move(_actions[CreatureActions.Run] ? _maxSpeed : _maxSpeed / 2f);

                if (_actions[CreatureActions.StrafeLeft] != _actions[CreatureActions.StrafeRight])
                    Move(_maxSpeed / 2f, _actions[CreatureActions.StrafeRight] ? MathHelper.ToRadians(90) : -MathHelper.ToRadians(90));
            }
            else if (_actions[CreatureActions.MoveBackward])
                    Move(_actions[CreatureActions.Run] ? -_maxSpeed/2f : -_maxSpeed/3f);

            var rotationSpeed = _maxRotatingSpeed;
            if (_actions[CreatureActions.RotateLeft] != _actions[CreatureActions.RotateRight])
            {
                if (_actions[CreatureActions.RotateLeft])
                    rotationSpeed = -_maxRotatingSpeed;
            }

            else rotationSpeed = 0;

            if (_actions[CreatureActions.MoveForward] && _actions[CreatureActions.Run])
                rotationSpeed *= 1.5f;
            Rotate(rotationSpeed);

            if (_actions[CreatureActions.Attack] && CurrentSprite != _attackSprite)
                CurrentSprite = _attackSprite;
            else if (!_actions[CreatureActions.Attack] && CurrentSprite != _normalSprite)
                CurrentSprite = _normalSprite;

        }

        //=======================================

        #endregion

        #region Actions

        //======================================


        //Movement

        private void Move(float vel, float angleShift = 0f)
        {
            
            var velocity = Utils.GetDirection(Body.Rotation + angleShift, ConvertUnits.ToSimUnits(vel));
            Body.Position += velocity;

            _speed = (float)Math.Sqrt(Math.Pow(velocity.X, 2) + Math.Pow(velocity.Y, 2)) / Size;

        }

        private void Rotate(float rotatingSpeed)
        {
            Body.Rotation += MathHelper.ToRadians(rotatingSpeed/Size);
        }

        private void Push(float force)
        {
            _acceleration += force;
        }

        
        //Life

        private void ProcessEating(float foodSize)
        {
            var foodSatiety = GameSettings.FoodSatiety*foodSize;
            if (_satiety + foodSatiety > 100f && _breedingSatiety + _satiety + foodSatiety - 100f < _breedingEnergy)
            {
                _breedingSatiety += _satiety + foodSatiety - 100f;
                _satiety = 100f;
                ProcessBreedingEnergy();
            }
            else _satiety = _satiety + foodSatiety;
            if (foodSatiety > _maxSatiety) _maxSatiety = foodSatiety;

            _neuralNetwork.Learn(true, GameSettings.NetworkLearningRate * foodSatiety / _maxSatiety);
        }

        private void Damage(float damage, bool attacked = false)
        {

            Health -= damage;
            _damageTimer = GameSettings.CreatureDamageDuration;
            if (attacked)
            {
                if (_maxDamage < damage)
                    _maxDamage = damage;
                _attackedDamage = damage / _maxDamage;
                //Push(-GameSettings.CreatureMovingSpeed * 2 / Size);
            }
            if (Health <= 0)
            {
                WorldProxy.Kill(this);
            }
            _neuralNetwork.Learn(false, GameSettings.NetworkLearningRate * _attackedDamage * GameSettings.NetworkDamageLearningRateAmplifier);

        }

        private void Heal(float value)
        {
            Health = Math.Min(Health + value, MaxHealth);
        }


        private void ProcessBreedingEnergy()
        {
            var energy = _breedingSatiety + _breedingTimer;
            if (!(energy >= _breedingEnergy)) return;
            _childrenToSpawn += Utils.Random.Next((int)Math.Max(1, GameSettings.CreatureChildrenMin * Size), (int)Math.Min(GameSettings.CreatureChildrenMax + 2, GameSettings.CreatureChildrenMax * Size));
            _breedingSatiety = _breedingTimer = 0;
        }



        //Watching


        private void Look()
        {
            //Raytracing in 5 directions (from -40 to 40 degrees)

            _lookDistance = Utils.InitList(0f, 5);
            _lookSize = Utils.InitList(0f, 5);
            _lookSpeed = Utils.InitList(0f, 5);
            _lookSpace = Utils.InitList(0f, 5);
            _lookRotation = Utils.InitList(0f, 5);
            _lookColor = Utils.InitList(Color.Black, 5);

            //left 40
            _currentLookingDirection = 0;
            _lookDirection = Utils.GetDirection(Body.Rotation - MathHelper.ToRadians(40), GameSettings.CreatureLookRange);
            WorldProxy.World.RayCast(RayTest, Body.Position, _lookDirection);
            LookForWall(_lookDirection);

            //left 20
            _currentLookingDirection = 1;
            _lookDirection = Utils.GetDirection(Body.Rotation - MathHelper.ToRadians(20), GameSettings.CreatureLookRange);
            WorldProxy.World.RayCast(RayTest, Body.Position, _lookDirection);
            LookForWall(_lookDirection);

            //front
            _currentLookingDirection = 2;
            _lookDirection = Utils.GetDirection(Body.Rotation, GameSettings.CreatureLookRange);
            WorldProxy.World.RayCast(RayTest, Body.Position, _lookDirection);
            LookForWall(_lookDirection);

            //right 20
            _currentLookingDirection = 3;
            _lookDirection = Utils.GetDirection(Body.Rotation + MathHelper.ToRadians(20), GameSettings.CreatureLookRange);
            WorldProxy.World.RayCast(RayTest, Body.Position, _lookDirection);
            LookForWall(_lookDirection);

            //right 40
            _currentLookingDirection = 4;
            _lookDirection = Utils.GetDirection(Body.Rotation + MathHelper.ToRadians(40), GameSettings.CreatureLookRange);
            WorldProxy.World.RayCast(RayTest, Body.Position, _lookDirection);
            LookForWall(_lookDirection);

        }

        //I see something!
        private float RayTest(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        {
            //I'm not interesing of it if it's not a creature of a food
            if ((string)fixture.Body.UserData == "creature" && !WorldProxy.EntityExists(fixture.Body)) return -1;
            if ((string)fixture.Body.UserData == "food" && !WorldProxy.EntityExists(fixture.Body)) return -1;

            _lookDistance[_currentLookingDirection] = Utils.GetDistance(_facePosition, point) / GameSettings.CreatureLookRange;
            
            var entity = WorldProxy.GetEntityInfo(fixture.Body);
            
            if (_maxSize < entity.Size)
                _maxSize = entity.Size;
            if (_maxTargetSpeed < entity.Speed)
                _maxTargetSpeed = entity.Speed;

            _lookSize[_currentLookingDirection] = entity.Size / _maxSize;
            _lookRotation[_currentLookingDirection] = Math.Abs(fixture.Body.Rotation - Utils.GetAimingAngle(point, Body.Position)) / MathHelper.ToRadians(180);
            _lookSpeed[_currentLookingDirection] = entity.Speed/_maxTargetSpeed;
            _lookColor[_currentLookingDirection] = entity.AverageColor;


            return fraction;
        }

        //If there's some wall?
        private void LookForWall(Vector2 direction)
        {

            var minDistance = -1f;
            var position = ConvertUnits.ToDisplayUnits(Body.Position);
            var point = position + ConvertUnits.ToDisplayUnits(direction);
            var maxDistance = Utils.GetDistance(position, point);
            var bounds = GameSettings.Borders;
            float distance;
            Vector2 wallPoint;

            if (point.X <= bounds.Left)
            {
                wallPoint = new Vector2(bounds.Left,
                    (position.X - bounds.Left)*(point.Y - position.Y)/(position.X - point.X) + position.Y);
                if (wallPoint.Y < bounds.Top)
                    wallPoint = new Vector2((position.Y - bounds.Top) * (point.X - position.X) / (position.Y - point.Y) + position.X,
                        bounds.Top);
                else if (wallPoint.Y > bounds.Bottom)
                    wallPoint = new Vector2(
                        (bounds.Bottom - position.Y) * (point.X - position.X) / (point.Y - position.Y) + position.X,
                        bounds.Bottom);

                distance = Utils.GetDistance(position, wallPoint);
                if (distance < minDistance || minDistance < 0) minDistance = distance;
            }
            else if (point.X >= bounds.Right)
            {
                wallPoint = new Vector2(bounds.Right,
                    (bounds.Right - position.X)*(point.Y - position.Y)/(point.X - position.X) + position.Y);
                if (wallPoint.Y < bounds.Top)
                    wallPoint = new Vector2((position.Y - bounds.Top) * (point.X - position.X) / (position.Y - point.Y) + position.X,
                        bounds.Top);
                else if (wallPoint.Y > bounds.Bottom)
                    wallPoint = new Vector2(
                        (bounds.Bottom - position.Y) * (point.X - position.X) / (point.Y - position.Y) + position.X,
                        bounds.Bottom);

                distance = Utils.GetDistance(position, wallPoint);
                if (distance < minDistance || minDistance < 0) minDistance = distance;
            }

            if (point.Y <= bounds.Top)
            {
                wallPoint =
                    new Vector2((position.Y - bounds.Top)*(point.X - position.X)/(position.Y - point.Y) + position.X,
                        bounds.Top);
                if (wallPoint.X < bounds.Left)
                    wallPoint = new Vector2(bounds.Left,
                        (position.X - bounds.Left) * (point.Y - position.Y) / (position.X - point.X) + position.Y);
                else if (wallPoint.X > bounds.Right)
                    wallPoint = new Vector2(bounds.Right,
                        (bounds.Right - position.X) * (point.Y - position.Y) / (point.X - position.X) + position.Y);

                distance = Utils.GetDistance(position, wallPoint);
                if (distance < minDistance || minDistance < 0) minDistance = distance;
            }
            else if (point.Y >= bounds.Bottom)
            {
                wallPoint =
                    new Vector2(
                        (bounds.Bottom - position.Y)*(point.X - position.X)/(point.Y - position.Y) + position.X,
                        bounds.Bottom);
                if (wallPoint.X < bounds.Left)
                    wallPoint = new Vector2(bounds.Left,
                        (position.X - bounds.Left) * (point.Y - position.Y) / (position.X - point.X) + position.Y);
                else if (wallPoint.X > bounds.Right)
                    wallPoint = new Vector2(bounds.Right,
                        (bounds.Right - position.X) * (point.Y - position.Y) / (point.X - position.X) + position.Y);

                distance = Utils.GetDistance(position, wallPoint);
                if (distance < minDistance || minDistance < 0) minDistance = distance;
            }

            if (minDistance < 0) minDistance = 0;

            _lookSpace[_currentLookingDirection] = 1 - minDistance/maxDistance;

        }

        //I can smell a lots of food!
        private void Sniff()
        {
            Utils.StartTest();

            _smellFoodDistance = 0;
            _smellCreatureDistance = 0;
            float maxDistance;

            var entities = WorldProxy.NearestEntities(this, _maxSmellDistance);
            var points = entities.FindAll(p => p.Name == "food").Select(s => s.Position).ToList();

            if (points.Count > 0)
            {
                maxDistance = points.Max(s => Utils.GetDistance(_facePosition, s));
                var distance = maxDistance;
                _smellFoodDistance = points.Average(s => Utils.GetDistance(_facePosition, s)/distance);
            }

            //Oh no, there's a creatures beside!
            points = entities.FindAll(p => p.Name == "creature").Select(s => s.Position).ToList();

            if (points.Count <= 0)
            {
                Utils.EndTest();
                return;
            }

            maxDistance = points.Max(s => Utils.GetDistance(_facePosition, s));
            _smellCreatureDistance = points.Average(s => Utils.GetDistance(_facePosition, s) / maxDistance);

            Utils.EndTest();
        }

        #endregion

        #region Network functions

        //=======================================


        private void ProcessNetwork()
        {
            //Setting network inputs

            float[] inputs =
                {
                    Health/MaxHealth,
                    _satiety/100f,
                    _speed/_maxSpeed,
                    _breedingSatiety/_breedingEnergy/2,
                    _breedingTimer/_breedingEnergy/2,
                    _attackedDamage,
                    _smellFoodDistance,
                    _smellCreatureDistance
                };

            var inputSensors = new List<float>();
            for (var i = 0; i < _lookDistance.Count; i++)
            {
                inputSensors.Add(_lookDistance[i]);
                inputSensors.Add(_lookRotation[i]);
                inputSensors.Add(_lookSize[i]);
                inputSensors.Add(_lookSpace[i]);
                inputSensors.Add(_lookSpeed[i]);
                inputSensors.Add(_lookColor[i].R/255f);
                inputSensors.Add(_lookColor[i].G/255f);
                inputSensors.Add(_lookColor[i].B/255f);
            } 

            //amplifying sensors inputs
            inputs = inputs.Concat(inputSensors.Select(s => s * GameSettings.NetworkSensorsAmplifier)).ToArray();

            //running the network
            var outputs = _neuralNetwork.Run(inputs).ToArray();

            //processing recieved information
            _actions[CreatureActions.MoveForward] = outputs[0] >= _networkThreshold;
            _actions[CreatureActions.MoveBackward] = outputs[1] >= _networkThreshold;
            _actions[CreatureActions.RotateLeft] = outputs[2] >= _networkThreshold;
            _actions[CreatureActions.RotateRight] = outputs[3] >= _networkThreshold;
            _actions[CreatureActions.Run] = outputs[4] >= _networkThreshold;
            _actions[CreatureActions.StrafeLeft] = outputs[5] >= _networkThreshold;
            _actions[CreatureActions.StrafeRight] = outputs[6] >= _networkThreshold;
            _actions[CreatureActions.Attack] = outputs[7] >= _networkThreshold;

        }


        //=======================================

        #endregion


        //Some actions that can be called from parent class
        public override void PerformAction(string action, List<object> parameters )
        {
            switch (action)
            {
                case "childspawned":
                    _childrenToSpawn--;
                    break;
                case "damage":
                    Damage((float)parameters[0], true);
                    break;
                default:
                    throw new ArgumentException("Action " + action + " not supported.");
            }
        }

        //And some creature-specified data
        public override Dictionary<string, object> GetCustomData()
        {
            return new Dictionary<string, object>
            {
                {"generation", _generation},
                {"damagetimer", _damageTimer},
                {"health", Health},
                {"maxhealth", MaxHealth},
                {"breedingsatiety", _breedingSatiety},
                {"breedingtimer", _breedingTimer},
                {"breedingenergy", _breedingEnergy},
                {"childrentospawn", _childrenToSpawn},
                {"faceposition", _facePosition},
                {"neuralnetwork", _neuralNetwork}
            };
        }
    }


    internal enum CreatureActions
    {
        MoveForward = 0,
        MoveBackward = 1,
        RotateLeft = 2,
        RotateRight = 3,
        Run = 4,
        StrafeLeft = 5,
        StrafeRight = 6,
        Attack = 7
    };
}
