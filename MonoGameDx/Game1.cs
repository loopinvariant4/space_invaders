using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        AnimatedSprite sprite;
        AnimatedSprite sprite2;
        AnimatedSprite bulletSprite;
        AnimatedSprite commanderSprite;
        AssetLoader loader;
        Queue<Tuple<int, GameObject>> addQueue = new Queue<Tuple<int, GameObject>>();
        Queue<Tuple<int, GameObject>> removeQueue = new Queue<Tuple<int, GameObject>>();
        AlienBag alienBag;
        Collider collider;

        public static Dictionary<int, GameObject> GameObjects = new Dictionary<int, GameObject>();
        Vector2 pos;
        SpriteFont font;
        int fps;
        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        Song song;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1500;
            graphics.PreferredBackBufferHeight = 700;
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

            collider = new Collider();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            loader = new AssetLoader(this.Content, spriteBatch);
            DIContainer.Add<AssetLoader>("AssetLoader", loader);
            DIContainer.Add<Dictionary<int, GameObject>>("GameObjects", GameObjects);
            DIContainer.Add<Queue<Tuple<int, GameObject>>>("AddQueue", addQueue);
            DIContainer.Add<Queue<Tuple<int, GameObject>>>("RemoveQueue", removeQueue);
            DIContainer.Add<Collider>("Collider", collider);

            alienBag = new AlienBag();
            DIContainer.Add<AlienBag>("AlienBag", alienBag);

            Env.Screen = this.GraphicsDevice.Viewport;
            base.Initialize();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            alienBag.GenerateLevel();
            GameObjects.Add(1, new Commander(1));

            font = Content.Load<SpriteFont>("courier");

            //this.song = Content.Load<Song>("Burning Brides - Artic Snow");
            //MediaPlayer.Play(song);

            // TODO: use this.Content to load your game content here
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            while(removeQueue.Count > 0)
            {
                var tup = removeQueue.Dequeue();
                GameObjects.Remove(tup.Item1);
            }
            while(addQueue.Count > 0)
            {
                var tup = addQueue.Dequeue();
                GameObjects.Add(tup.Item1, tup.Item2);
            }

            collider.CheckCollisions();
            alienBag.CheckEdgeCollision();

            foreach(var obj in GameObjects.Values)
            {
                obj.OnInput();
                obj.Update(gameTime);
            }

            collider.RemoveQueuedItems();
            // TODO: Add your update logic here
            //sprite.Position.X = (sprite.Position.X + 1) % this.GraphicsDevice.Viewport.Width;
            //sprite2.Position.X = (sprite2.Position.X + 1) % this.GraphicsDevice.Viewport.Width;

            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > System.TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
            //sprite.Update(gameTime);
            //sprite2.Update(gameTime);
            
            base.Update(gameTime);
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            frameCounter++;
            spriteBatch.Begin();
            spriteBatch.DrawString(font, frameRate.ToString(), new Vector2(0, 0), Color.White);
            //sprite.Draw(spriteBatch);
            //sprite2.Draw(spriteBatch);
            foreach(var obj in GameObjects.Values)
            {
                obj.Draw(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
