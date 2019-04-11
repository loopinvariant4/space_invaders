using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SI
{
    public class Commander : GameObject
    {
        Bullet bullet = null;
        AnimatedSprite commander = null;
        Dictionary<int, GameObject> gameObjects;
        Queue<Tuple<int, GameObject>> addQueue;
        Queue<Tuple<int, GameObject>> removeQueue;
        private State current = State.Alive;
        private int bulletCount = 0;
        private readonly TimeSpan FIREINTERVAL = TimeSpan.FromMilliseconds(500); //ms
        TimeSpan lastFireTime = TimeSpan.Zero;


        public override AnimatedSprite Sprite => commander;

        public override Rectangle Box => Sprite.Box;

        public override State Current => current;

        public Commander(int id)
        {
            commander = new AnimatedSprite("commander", 3, 1, 12, true, Point.Zero, null);
            commander.Position = new Point(Env.Screen.Width / 2 + commander.Width / 2, Env.Screen.Height - 100);
            Id = id;
            gameObjects = DIContainer.Get<Dictionary<int, GameObject>>("GameObjects");
            addQueue = DIContainer.Get<Queue<Tuple<int, GameObject>>>("AddQueue");
            removeQueue = DIContainer.Get<Queue<Tuple<int, GameObject>>>("RemoveQueue");

        }

        public void Fire(GameTime gt)
        {
            if (bullet == null)
            {
                if (gt.TotalGameTime - lastFireTime > FIREINTERVAL)
                {
                    createBullet();
                    Console.WriteLine("Bullets fired: {0}", ++bulletCount);
                    lastFireTime = gt.TotalGameTime;
                }
            }
        }

        private void createBullet()
        {
            bullet = new Bullet(new Point(commander.Position.X + commander.Width / 2, commander.Position.Y - commander.Height / 2), IdGen.Next);
            bullet.Destroyed += Bullet_Destroyed;
            addQueue.Enqueue(new Tuple<int, GameObject>(bullet.Id, bullet));
        }

        private void Bullet_Destroyed(object sender, EventArgs e)
        {
            bullet.Destroyed -= Bullet_Destroyed;
            removeQueue.Enqueue(new Tuple<int, GameObject>(bullet.Id, bullet));
            bullet = null;
        }

        public override void Update(GameTime gt)
        {
            commander.Update(gt);
        }

        public override void Draw(SpriteBatch batch)
        {
            commander.Draw(batch);
        }

        public override void OnInput(GameTime gt)
        {
            if(Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                commander.Position.Y -= 2;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                commander.Position.Y += 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                commander.Position.X -= 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                commander.Position.X += 2;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Fire(gt);
            }
        }

        public override void Destroy()
        {
            throw new NotImplementedException();
        }

        public override void CollidedWith(GameObject otherObject)
        {
            throw new NotImplementedException();
        }
    }
}
