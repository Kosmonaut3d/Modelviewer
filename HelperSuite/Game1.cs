using System;
using System.Runtime.InteropServices;
using HelperSuite.HelperSuite.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OceanRender.Main;

namespace HelperSuite
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch spriteBatch;

        private ScreenManager _screenManager;
        private bool _isActive = true;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _screenManager = new ScreenManager();

            //Set up graphics properties, no vsync, no framelock
            _graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = true;
            //TargetElapsedTime = TimeSpan.FromMilliseconds(100);

            //Size of our application / starting back buffer
            _graphics.PreferredBackBufferWidth = GameSettings.g_ScreenWidth;
            _graphics.PreferredBackBufferHeight = GameSettings.g_ScreenHeight;

            _graphics.PreferMultiSampling = false;
            _graphics.PreferredBackBufferFormat = SurfaceFormat.Color;

            //_graphics.PreferredDepthStencilFormat = DepthFormat.Depth24;
            //HiDef enables usable shaders
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            _graphics.ApplyChanges();

            //Mouse should not disappear
            IsMouseVisible = true;

            //Window settings
            Window.AllowUserResizing = true;
            Window.IsBorderless = false;

            //Update all our rendertargets when we resize
            Window.ClientSizeChanged += ClientChangedWindowSize;

            //Update framerate etc. when not the active window
            Activated += IsActivated;
            Deactivated += IsDeactivated;
        }
        private void IsActivated(object sender, EventArgs e)
        {
            _isActive = true;
        }

        private void IsDeactivated(object sender, EventArgs e)
        {
            _isActive = false;
        }

        /// <summary>
        /// Update rendertargets and backbuffer when resizing window size
        /// </summary>
        private void ClientChangedWindowSize(object sender, EventArgs e)
        {
            if (GraphicsDevice.Viewport.Width != _graphics.PreferredBackBufferWidth ||
                GraphicsDevice.Viewport.Height != _graphics.PreferredBackBufferHeight)
            {
                if (Window.ClientBounds.Width == 0) return;
                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                _graphics.ApplyChanges();

                GameSettings.g_ScreenWidth = Window.ClientBounds.Width;
                GameSettings.g_ScreenHeight = Window.ClientBounds.Height;

                _screenManager.UpdateResolution();
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _screenManager.Load(Content, GraphicsDevice);
            
            _screenManager.Initialize(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            _screenManager.Unload(Content);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                UnloadContent();
                Exit();
            }
            // TODO: Add your update logic here

            _screenManager.Update(gameTime, _isActive);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(GameSettings.testvaluer, GameSettings.testvalueg, GameSettings.testvalueb));

            _screenManager.Draw(gameTime);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
