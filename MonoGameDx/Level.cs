using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SI
{
    /// <summary>
    /// Manage the levels in a game. For each level, this class manages all aspects of the game from the drawing and update loops to gameobjects and the 
    /// logic for the gameplay as a whole
    /// </summary>
    public class Level : IGameStage
    {
        #region vars
        private Collider collider;
        private AlienBag alienBag;
        private Queue<Tuple<int, GameObject>> addQueue = new Queue<Tuple<int, GameObject>>();
        private Queue<Tuple<int, GameObject>> removeQueue = new Queue<Tuple<int, GameObject>>();
        public Dictionary<int, GameObject> GameObjects = new Dictionary<int, GameObject>();
        private Commander commander = null;
        public event EventHandler LevelComplete;
        private SpriteFont font;
        private int score = 0;
        private int lives = 4;
        private List<AnimatedSprite> livesSprites = new List<AnimatedSprite>();
        private int currentLevel = 0;
        private bool isAlienVictorious = false;
        public event EventHandler End;
        #endregion


        #region ctor
        public Level(string id)
        {
            Id = id;
        }
        #endregion

        #region functions
        public IGameStage Next { get; set; }

        public string Id { get; }

        /// <summary>
        /// Starts a new level with a fresh set of aliens. 
        /// </summary>
        /// <param name="level">Determines level difficulty</param>        
        private void Start(int level)
        {
            // TODO: ignore level difficulty for now and go with default values
            alienBag.GenerateLevel();
            addLivesSprites();
            addCommander();
        }

        public void NextLevel(int level)
        {
            // TODO: ignore level difficulty for now and go with default values
            alienBag.GenerateLevel();
            registerCommanderWithAliens();
        }



        #region IGameStage interface
        public void BeforeStart(GameStageSettings settings = null)
        {
            ResetGame();
        }



        private void ResetGame()
        {
            lives = 4;
            score = 0;
            DIContainer.Add<Dictionary<int, GameObject>>("GameObjects", GameObjects);
            DIContainer.Add<Queue<Tuple<int, GameObject>>>("AddQueue", addQueue);
            DIContainer.Add<Queue<Tuple<int, GameObject>>>("RemoveQueue", removeQueue);

            collider = new Collider();
            DIContainer.Add<Collider>("Collider", collider);
            alienBag = new AlienBag();
            DIContainer.Add<AlienBag>("AlienBag", alienBag);
            alienBag.AllDead += (o, e) => NextLevel(currentLevel++);
            alienBag.AlienDead += (o, e) => updateScore(e);
            alienBag.AlienVictory += (o, e) =>
            {
                isAlienVictorious = true;
                commander.Kill();
            };
            font = DIContainer.Get<AssetLoader>("AssetLoader").Content.Load<SpriteFont>("courier");
        }

        public void Start()
        {
            Start(currentLevel++);
        }

        public void Update(GameTime gameTime, GameInput input)
        {
            while (removeQueue.Count > 0)
            {
                Tuple<int, GameObject> tup = removeQueue.Dequeue();
                GameObjects.Remove(tup.Item1);
            }
            while (addQueue.Count > 0)
            {
                Tuple<int, GameObject> tup = addQueue.Dequeue();
                GameObjects.Add(tup.Item1, tup.Item2);
            }

            if (isAlienVictorious == false)
            {
                collider.CheckCollisions();
                alienBag.CheckEdgeCollision();
            }

            foreach (GameObject obj in GameObjects.Values)
            {
                obj.OnInput(gameTime, input);
                obj.Update(gameTime);
                if (input.IsKeyDown(Keys.K))
                {
                    isAlienVictorious = true;
                    commander.Kill();
                }
            }
            collider.RemoveQueuedItems();
        }



        public void Draw(GameTime gt, SpriteBatch spriteBatch)
        {
            foreach (GameObject obj in GameObjects.Values)
            {
                obj.Draw(spriteBatch);
            }
            var scoreString = "Score: " + score;
            spriteBatch.DrawString(font, scoreString, new Vector2(0, 0), Color.White);

            foreach (var item in livesSprites)
            {
                item.Draw(spriteBatch);
            }

        }

        public void BeforeEnd()
        {
            DIContainer.Remove("GameObjects");
            DIContainer.Remove("AddQueue");
            DIContainer.Remove("RemoveQueue");
            DIContainer.Remove("Collider");
            DIContainer.Remove("AlienBag");
        }

        #endregion

        #endregion

        #region private functions
        private void addCommander()
        {
            Debug.Assert(commander == null, "This method should not be called when the commander is not null i.e. commander is still alive");
            Debug.Assert(lives != 0, "The code should not reach addCommander when lives is 0");

            int id = IdGen.Next;
            commander = new Commander(id);
            EventHandler evt = null;
            evt = (object o, EventArgs e) =>
            {
                commander.Destroyed -= evt;
                commander = null;
                if (lives == 0)
                {
                    BeforeEnd();
                    End?.Invoke(this, null);
                    return;
                }
                addCommander();
            };
            commander.Destroyed += evt;
            collider.Register(commander, alienBag.Aliens.Values.ToArray());
            addQueue.Enqueue(new Tuple<int, GameObject>(id, commander));
            lives--;
            livesSprites.RemoveAt(livesSprites.Count - 1);
            if (isAlienVictorious)
            {
                commander.Kill();
            }
        }



        private void updateScore(AlienDestroyedEventArgs e)
        {
            score += e.Score;
        }
        private void addLivesSprites()
        {
            int y = 0;
            int x = Env.Screen.Width;
            for (int i = 0; i < lives; i++)
            {
                var sprite = new AnimatedSprite("commander", 3, 1, 12, true, Point.Zero, null);
                sprite.Position.Y = y;
                sprite.Position.X = x - sprite.Width;
                x = sprite.Position.X;
                livesSprites.Add(sprite);
            }
        }

        private void registerCommanderWithAliens()
        {
            collider.Register(commander, alienBag.Aliens.Values.ToArray());
        }


        #endregion
    }
}
