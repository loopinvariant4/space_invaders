using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Game1
{
    public class Alien : GameObject, IState
    {
        private AnimatedSprite alien;
        private AnimatedSprite explosion;
        private AnimatedSprite currentSprite;
        private readonly Dictionary<int, GameObject> gameObjects;
        private readonly Queue<Tuple<int, GameObject>> addQueue;
        private Queue<Tuple<int, GameObject>> removeQueue;
        private AlienBag bag;
        private State current = State.Alive;

        public event EventHandler<CollisionEventArgs> OnCollide;

        public override AnimatedSprite Sprite => currentSprite;

        public int Direction { get; set; }

        public override Rectangle Box => Sprite.Box;

        public override State Current
        {
            get => current;
        }

        public Alien(int id, string alienKey, Point startposition, AlienBag bag)
        {
            alien = new AnimatedSprite(alienKey, 2, 1, 1, true, startposition, null);
            alien.Show();
            explosion = new AnimatedSprite("alien_explosion", 8, 1, 200, false, Point.Zero, () => Destroy());
            explosion.Hide();
            currentSprite = alien;
            Id = id;
            gameObjects = DIContainer.Get<Dictionary<int, GameObject>>("GameObjects");
            addQueue = DIContainer.Get<Queue<Tuple<int, GameObject>>>("AddQueue");
            removeQueue = DIContainer.Get<Queue<Tuple<int, GameObject>>>("RemoveQueue");
            this.bag = bag;
            Direction = 1;
        }

        public override void Draw(SpriteBatch batch)
        {
            Sprite.Draw(batch);
        }

        public override void OnInput()
        {
        }

        public override void Update(GameTime gt)
        {
            Sprite.Position.X = (alien.Position.X + Direction);
            Sprite.Update(gt);
        }

        public override void Destroy()
        {
            Goto(State.Dead);
            explosion.Hide();
            removeQueue.Enqueue(new Tuple<int, GameObject>(Id, this));
            bag.Remove(Id);
        }

        public override void CollidedWith(GameObject otherObject)
        {
            Goto(State.Dying);
            currentSprite = explosion;
            explosion.Position = alien.Position;
            alien.Hide();
            explosion.Show();
        }

        public void Goto(State next)
        {
            current = next;
        }
    }
}
