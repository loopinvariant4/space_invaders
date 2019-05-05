using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SI
{
    /// <summary>
    /// This class holds the full set of aliens for each level and performs operations that relate to the aliens as a group. 
    /// For now, this is the best way i could think of managing a group of GameObjects who are related and have to respond to a 
    /// single event as a group e.g. when they reach the right wall, all of them have to move left
    /// </summary>
    public class AlienBag
    {
        #region vars
        int cols = 10;
        const int XOFFSET = 50;
        const int YOFFSET = 100;
        const int MAXWIDTH = 72;
        const int MAXHEIGHT = 96;
        const int BOUNDARY = 10;
        const int DESCENDAMOUNT = 20;

        string[] keys = new string[] { "bugeye_alien", "pointy_alien", "antenna_alien", "flex_alien", "flex_alien" };
        Dictionary<int, GameObject> gameObjects;
        int alienDirection = 1;

        // reference to the global queue responsible for adding a game object into the renderer list
        private readonly Queue<Tuple<int, GameObject>> addQueue;

        Dictionary<int, Alien> aliens = new Dictionary<int, Alien>();
        public event EventHandler AllDead;
        #endregion

        public Dictionary<int, Alien> Aliens { get { return aliens; } }

        public AlienBag()
        {
            gameObjects = DIContainer.Get<Dictionary<int, GameObject>>("GameObjects");
            addQueue = DIContainer.Get<Queue<Tuple<int, GameObject>>>("AddQueue");

        }

        public void GenerateLevel()
        {
            int rowX = XOFFSET;
            int rowY = YOFFSET;
            var rowPos = Point.Zero;
            foreach (var r in Enumerable.Range(0, keys.Length))
            {
                rowPos.X = 0;
                rowPos.Y += r == 0 ? YOFFSET : aliens.Values.ToArray()[aliens.Count - 1].Sprite.Height + 10;
                foreach (var c in Enumerable.Range(0, cols))
                {
                    var id = IdGen.Next;
                    Alien a = new Alien(id, keys[r], rowPos, this);
                    aliens.Add(id, a);
                    addQueue.Enqueue(new Tuple<int, GameObject>(id, a));
                    rowPos.X += MAXWIDTH + 5;
                }
            }
        }

        public void Remove(int id)
        {
            aliens.Remove(id);
            if(aliens.Count % 10 == 0)
            {
                increaseAlienSpeed();
            }
            if(aliens.Count == 0)
            {
                AllDead?.Invoke(this, null);
            }
        }

        private void increaseAlienSpeed()
        {
            foreach(var alien in aliens.Values)
            {
                alien.Speed *= 2;
                alien.FrameRate += 1;
            }
        }

        public void CheckEdgeCollision()
        {
            bool shouldChangeDirection = false;
            foreach (var alien in aliens.Values)
            {
                if (alienDirection == 1)
                {
                    if (alien.Box.Right >= (Env.Screen.Width - BOUNDARY))
                    {
                        shouldChangeDirection = true;
                        break;
                    }
                }
                else
                {
                    if (alien.Box.Left <= BOUNDARY)
                    {
                        shouldChangeDirection = true;
                        break;
                    }
                }
            }
            if (shouldChangeDirection)
            {
                alienDirection *= -1;
                foreach (var alien in aliens.Values)
                {
                    alien.Direction = alienDirection;
                    alien.Sprite.Position.Y += DESCENDAMOUNT;
                }
            }
        }
    }
}
