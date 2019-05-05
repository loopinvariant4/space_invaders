using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SI
{
    /// <summary>
    /// Manage the levels in a game. For each level, this class manages all aspects of the game from the drawing and update loops to gameobjects and the 
    /// logic for the gameplay as a whole
    /// </summary>
    public class Level
    {
        #region vars
        private readonly Collider collider;
        private AlienBag alienBag;
        private Queue<Tuple<int, GameObject>> addQueue = new Queue<Tuple<int, GameObject>>();
        private Queue<Tuple<int, GameObject>> removeQueue = new Queue<Tuple<int, GameObject>>();
        public static Dictionary<int, GameObject> GameObjects = new Dictionary<int, GameObject>();
        private Commander commander = null;
        public event EventHandler LevelComplete;
        #endregion

        #region ctor
        public Level()
        {
            DIContainer.Add<Dictionary<int, GameObject>>("GameObjects", GameObjects);
            DIContainer.Add<Queue<Tuple<int, GameObject>>>("AddQueue", addQueue);
            DIContainer.Add<Queue<Tuple<int, GameObject>>>("RemoveQueue", removeQueue);

            collider = new Collider();
            DIContainer.Add<Collider>("Collider", collider);
            alienBag = new AlienBag();
            DIContainer.Add<AlienBag>("AlienBag", alienBag);
            alienBag.AllDead += (o, e) => LevelComplete?.Invoke(this, null);
        }
        #endregion

        #region functions



        /// <summary>
        /// Starts a new level with a fresh set of aliens. 
        /// </summary>
        /// <param name="level">Determines level difficulty</param>        
        public void StartLevel(int level)
        {
            // TODO: ignore level difficulty for now and go with default values
            alienBag.GenerateLevel();
            addCommander();
            collider.Register(commander, alienBag.Aliens.Values.ToArray());
        }

        /// <summary>
        /// Perform all the updates for all game objects in this level
        /// </summary>
        /// <param name="gt"></param>
        public void Update(GameTime gameTime)
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

            collider.CheckCollisions();
            alienBag.CheckEdgeCollision();

            foreach (GameObject obj in GameObjects.Values)
            {
                obj.OnInput(gameTime);
                obj.Update(gameTime);
            }

            collider.RemoveQueuedItems();
        }

        public void Draw(GameTime gt, SpriteBatch spriteBatch)
        {
            foreach (GameObject obj in GameObjects.Values)
            {
                obj.Draw(spriteBatch);
            }
        }
        #endregion

        #region private functions
        private void addCommander()
        {
            if (commander != null) return;

            int id = IdGen.Next;
            commander = new Commander(id);
            EventHandler evt = null;
            evt = (object o, EventArgs e) =>
            {
                commander.Destroyed -= evt;
                commander = null;
                addCommander();
            };
            commander.Destroyed += evt;
            addQueue.Enqueue(new Tuple<int, GameObject>(id, commander));
        }
        #endregion
    }
}
