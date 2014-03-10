using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Neurolution.Factories;
using Neurolution.Graphics;
using Neurolution.Helpers;
using Neurolution.Managers;

namespace Neurolution
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game, IActionObserver
    {
        //There is no necessity to make fields private becouse they're can't be called from anywhere


        //GraphicsDeviceManager is required for graphics initization.
        public readonly GraphicsDeviceManager Graphics;

        //You can find description of managers below in their files
        public ScreenManager ScreenManager;
        public ControlsManager ControlsManager;
        public GraphicsManager GraphicsManager;
        public MenuManager MenuManager;
        public WorldManager WorldManager;
        public List<IManager> Managers = new List<IManager>();

        //Used for methods that require GameTime instance and not called from Update() or Draw()
        public GameTime LastGameTime;


        
        public Game()
        {
            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 3 * 2,
                PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 3 * 2,
                PreferMultiSampling = true,
            };
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }


        //Classes below called from XNA base classes.
        //It was made for logical isolation of the game functions from framework methods

        public void GameInitialize()
        {
            //Loading configuration from file...
            var config = new Configuration(GameSettings.ConfigurationFile);

            //And writing it to static class GameSettings
            GameSettings.Load(config);

            //Initizating managers
            Managers.Add(ScreenManager = new ScreenManager());
            Managers.Add(ControlsManager = new ControlsManager());
            Managers.Add(GraphicsManager = new GraphicsManager(Graphics, GraphicsDevice, Window));
            Managers.Add(MenuManager = new MenuManager());
            Managers.Add(WorldManager = new WorldManager());
        }

        public void GameLoadContent()
        {
            //Post-initization (e.g. loading game content)
            var spriteFactory = new SpriteFactory(GraphicsManager);

            GraphicsManager.LoadContent(Content);
            MenuManager.LoadContent(
                new MenuPainter(GraphicsManager),
                new MenuFactory(GraphicsManager));
            WorldManager.LoadContent(spriteFactory,
                new WorldPainter(GraphicsManager));
            ScreenManager.LoadContent(GraphicsManager.Camera);

            //Registering listeners of ControlManager
            foreach (var manager in Managers)
                manager.RegisterControls(ControlsManager);

            //And providing an ability to ask Game Class to do something
            foreach (var manager in Managers)
                manager.Register(this);
        }


        public void GameUpdate(GameTime gameTime)
        {
            ScreenManager.Update(gameTime);
            ControlsManager.Update(gameTime);
        }

        public void GameDraw()
        {
            GraphicsManager.Clear();
            ScreenManager.Draw(LastGameTime);
        }


        //Updating background in GameMenu
        public void UpdateMenuBackground()
        {
            if (MenuManager.CurrentMenuName() != "gamemenu") return;
            var background = GraphicsManager.GameScreenshot(0.5f);
            MenuManager.SetBackground(background);
        }

        //Here may be made filtering (accepting/denying) of incoming requests
        public void RequestAction(string action, List<object> parameters )
        {
            PerformAction(action, parameters, LastGameTime);
        }

        //Processing requests
        public void PerformAction(string action, List<object> parameters, GameTime gameTime)
        {
            switch (action)
            {
                case "pause":
                    UpdateMenuBackground();
                    MenuManager.SetMenu(1);
                    ScreenManager.SetScreen(0, false);
                    IsMouseVisible = true;
                    break;
                case "resume":
                    MenuManager.Leave();
                    ScreenManager.SetScreen(1, false);
                    IsMouseVisible = GameSettings.DisplayCursor;
                    break;
                case "init":
                    Managers.Find(p => p.GetName() == (string)parameters[0]).Initialize();
                    break;
                case "draw":
                    if(parameters.Count == 0)
                        ScreenManager.Draw(gameTime);
                    else Managers.Find(p => p.GetName() == (string)parameters[0]).Draw(gameTime);
                    break;
                case "drawworld":
                    WorldManager.Draw(gameTime, (bool)parameters[0]);
                    break;
                case "update":
                    Managers.Find(p => p.GetName() == (string)parameters[0]).Update(gameTime);
                    break;
                case "newgame":
                    MenuManager.Leave();
                    ScreenManager.SetScreen(1, true);
                    IsMouseVisible = GameSettings.DisplayCursor;
                    break;
                case "exit":
                    Exit();
                    break;
                case "resize":
                    MenuManager.Resize();
                    break;
                case "menubackground":
                    UpdateMenuBackground();
                    break;
                case "fullscreen":
                    Graphics.ToggleFullScreen();
                    MenuManager.Resize();
                    break;
                case "screenshot":
                    GraphicsManager.TakeScreenshot();
                    break;
                default:
                    throw new Exception("Action " + action + " is not supported.");
            }
        }



        // ===========================



        //These functions is provided (and then called) by XNA framework.
        #region Native XNA functions

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GameInitialize();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            GameLoadContent();
        }
/*
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }
        */
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            LastGameTime = gameTime;
            GameUpdate(gameTime);
            
            //foreach(var action in ActionsToExecute)
            //    PerformAction(action.Key, action.Value, gameTime);
            //ActionsToExecute.Clear();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            LastGameTime = gameTime;
            GameDraw();
            base.Draw(gameTime);

        }


        // ===========================

        #endregion
    }
}
