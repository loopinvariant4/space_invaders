using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace SI
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private IGameStage currentStage;
        private AssetLoader loader;

        private Vector2 pos;
        private SpriteFont font;
        private readonly int fps;
        private int frameRate = 0;
        private int frameCounter = 0;
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private readonly Song song;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
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

            this.IsMouseVisible = true;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            loader = new AssetLoader(this.Content, spriteBatch);
            //graphics.PreferredBackBufferWidth = this.GraphicsDevice.Viewport.Width;
            //graphics.PreferredBackBufferHeight = this.GraphicsDevice.Viewport.Height;

            Env.Screen = this.GraphicsDevice.Viewport;
            base.Initialize();
        }



        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>("courier");

        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void BeginRun()
        {
            startNewGame();
            base.BeginRun();
        }



        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            currentStage.Update(gameTime);

            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime > System.TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }

            base.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Clear(Color.Black);


            // TODO: Add your drawing code here
            frameCounter++;
            var fontSize = font.MeasureString(frameRate.ToString());
            spriteBatch.Begin();
            spriteBatch.DrawString(font, frameRate.ToString(), new Vector2(Env.Screen.Width - fontSize.Length(), 0), Color.White);
            currentStage.Draw(gameTime, spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        #region private methods
        /// <summary>
        /// Create all the stages of the game and wire them up to load one after the other
        /// </summary>
        private IGameStage createGameStages()
        {
            var stage1 = new MainScreen("MainScreen");
            var stage2 = new Level("Level");
            var stage3 = new GameOver("GameOver");

            stage1.Next = stage2;
            stage2.Next = stage3;
            stage3.Next = stage1;

            stage1.End += (o, e) => { currentStage = stage2; currentStage.BeforeStart(); currentStage.Start(); };
            stage2.End += (o, e) => { currentStage = stage3; currentStage.BeforeStart(); currentStage.Start(); };
            stage3.End += (o, e) => { startNewGame(); };
            return stage1;
        }

        private void startNewGame()
        {
            DIContainer.Clear();
            DIContainer.Add<AssetLoader>("AssetLoader", loader);
            currentStage = createGameStages();
            currentStage.BeforeStart();
            currentStage.Start();
        }
        #endregion
    }
}
