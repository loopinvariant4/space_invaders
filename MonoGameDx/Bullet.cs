using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SI
{
    public class Bullet : GameObject
    {
        AnimatedSprite bulletSprite = null;
        public event EventHandler Destroyed;
        public event EventHandler Collision;
        private int SPEED = 5; // pixels per second
        private State current = State.Alive;
        public override AnimatedSprite Sprite => bulletSprite;

        public override Rectangle Box => Sprite.Box;

        public Point Position
        {
            get
            {
                return bulletSprite.Position;
            }
        }

        public override State Current => current;

        public Bullet(Point startPosition, int id)
        {
            bulletSprite = new AnimatedSprite("bullet", 1, 1, 1, true, startPosition, null);
            bulletSprite.Position.X -= bulletSprite.Width / 2;
            var collider = DIContainer.Get<Collider>("Collider");
            var aliens = DIContainer.Get<AlienBag>("AlienBag");
            collider.Register(this, aliens.Aliens.Values.ToArray());
            Id = id;
        }

        private void checkPosition()
        {
            if (isOutOfBounds())
            {
                Destroy();
            }
        }

        public override void Destroy()
        {
            current = State.Dead;
            Destroyed?.Invoke(this, null);
        }

        private bool isOutOfBounds()
        {
            return this.Position.Y <= 0;
        }

        private bool hasCollision()
        {
            return false;
        }

        public override void Update(GameTime gt)
        {
            bulletSprite.Position.Y -= SPEED;
            checkPosition();
        }

        public override void Draw(SpriteBatch batch)
        {
            bulletSprite.Draw(batch);
        }

        public override void OnInput(GameTime gt)
        {
        }

        public override void CollidedWith(GameObject otherObject)
        {
            current = State.Dying;
            DIContainer.Get<Collider>("Collider").UnRegister(this);
            Collision?.Invoke(this, null);
            Destroy();
        }
    }
}
