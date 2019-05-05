using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SI
{
    public class Commander : GameObject
    {
        private Bullet bullet = null;
        private AnimatedSprite commanderSprite = null;
        private readonly Dictionary<int, GameObject> gameObjects;
        private Queue<Tuple<int, GameObject>> addQueue;
        private Queue<Tuple<int, GameObject>> removeQueue;
        private readonly State current = State.Alive;
        private int bulletCount = 0;
        private readonly TimeSpan FIREINTERVAL = TimeSpan.FromMilliseconds(500); //ms
        private TimeSpan lastFireTime = TimeSpan.Zero;
        private readonly bool isInputEnabled = true;
        public event EventHandler Destroyed;


        // sprite for the explosion when an commander dies
        private AnimatedSprite explosionSprite;

        // reference to the current sprite which is determined by the state the commander is in
        private AnimatedSprite currentSprite;

        // references the current sprite based on the state of the alien
        public override AnimatedSprite Sprite => currentSprite;


        public override Rectangle Box => Sprite.Box;

        public override State Current => current;

        public Point Position => commanderSprite.Position;

        public Commander(int id)
        {
            commanderSprite = new AnimatedSprite("commander", 3, 1, 12, true, Point.Zero, null);
            explosionSprite = new AnimatedSprite("commander_explosion", 7, 1, 21, false, Point.Zero, () => Destroy());
            explosionSprite.Hide();

            currentSprite = commanderSprite;

            commanderSprite.Position = new Point(Env.Screen.Width / 2 + commanderSprite.Width / 2, Env.Screen.Height - 100);
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
            bullet = new Bullet(new Point(commanderSprite.Position.X + commanderSprite.Width / 2, commanderSprite.Position.Y - commanderSprite.Height / 2), IdGen.Next);
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
            Sprite.Update(gt);
        }

        public override void Draw(SpriteBatch batch)
        {
            Sprite.Draw(batch);
        }

        public override void OnInput(GameTime gt)
        {
            int moveSpeed = 3;
            if (isInputEnabled == false) return;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (isOutOfBounds(Bounds.Top) == false)
                {
                    commanderSprite.Position.Y -= moveSpeed;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (isOutOfBounds(Bounds.Bottom) == false)
                {
                    commanderSprite.Position.Y += moveSpeed;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                if (isOutOfBounds(Bounds.Left) == false)
                {
                    commanderSprite.Position.X -= moveSpeed;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                if (isOutOfBounds(Bounds.Right) == false)
                {
                    commanderSprite.Position.X += moveSpeed;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Fire(gt);
            }
        }

        public override void CollidedWith(GameObject otherObject)
        {
            DIContainer.Get<Collider>("Collider").UnRegister(this);
            currentSprite = explosionSprite;
            explosionSprite.Position = commanderSprite.Position;
            commanderSprite.Hide();
            explosionSprite.Show();
        }

        public override void Destroy()
        {
            explosionSprite.Hide();
            Destroyed?.Invoke(this, null);
            removeQueue.Enqueue(new Tuple<int, GameObject>(Id, this));
        }

        private bool isOutOfBounds(Bounds bounds)
        {
            switch (bounds)
            {
                case Bounds.Top:
                    return this.Position.Y <= 0;
                case Bounds.Left:
                    return this.Position.X <= 0;
                case Bounds.Bottom:
                    return this.Position.Y >= Env.Screen.Height - this.commanderSprite.Height;
                case Bounds.Right:
                    return this.Position.X >= Env.Screen.Width - this.commanderSprite.Width;
            }
            return false;
        }
    }

    public enum Bounds
    {
        Top,
        Left,
        Bottom,
        Right
    }
}
