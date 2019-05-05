using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SI
{
    /// <summary>
    /// Alien is an entity class that holds the behaviors of a single alien - from animations, sounds and state
    /// </summary>
    public class Alien : GameObject, IState
    {
        #region vars
        // sprite for the alien
        private AnimatedSprite alien;   

        // sprite for the explosion when an alien dies
        private AnimatedSprite explosion;

        // reference to the current sprite which is determined by the state the alien is in
        private AnimatedSprite currentSprite;

        // reference to the global list of game objects used by the main game loop
        private readonly Dictionary<int, GameObject> gameObjects;

        // reference to the global queue responsible for adding a game object into the renderer list
        private readonly Queue<Tuple<int, GameObject>> addQueue;

        // reference to the global queue responsible for removing a game object from the renderer list
        private Queue<Tuple<int, GameObject>> removeQueue;

        // reference to the container class of aliens responsible for group behaviors
        private AlienBag bag;

        // current state of this game object
        private State current = State.Alive;

        // list of sounds that this alien can hold
        Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();
        #endregion

        #region properties
        // references the current sprite based on the state of the alien
        public override AnimatedSprite Sprite => currentSprite;

        // determines if the alien moves left(-1) or right(+1)
        public int Direction { get; set; }

        // determines the speed of the alien
        public int Speed { get; set; }

        public int FrameRate
        {
            get => alien.FrameRate;
            set => alien.FrameRate = value;
        }

        public override Rectangle Box => Sprite.Box;

        public override State Current
        {
            get => current;
        }
        #endregion

        #region ctor
        /// <summary>
        /// Initialize an alien with an id, key to load sprite, startingposition and the container for all aliens 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="alienKey"></param>
        /// <param name="startposition"></param>
        /// <param name="bag"></param>
        public Alien(int id, string alienKey, Point startposition, AlienBag bag)
        {
            alien = new AnimatedSprite(alienKey, 2, 1, 1, true, startposition, null);
            alien.Show();
            explosion = new AnimatedSprite("alien_explosion", 8, 1, 1, false, Point.Zero, () => Destroy());
            explosion.Hide();
            currentSprite = alien;
            Id = id;
            gameObjects = DIContainer.Get<Dictionary<int, GameObject>>("GameObjects");
            addQueue = DIContainer.Get<Queue<Tuple<int, GameObject>>>("AddQueue");
            removeQueue = DIContainer.Get<Queue<Tuple<int, GameObject>>>("RemoveQueue");
            this.bag = bag;

            sounds.Add("tick", DIContainer.Get<AssetLoader>("AssetLoader").Content.Load<SoundEffect>("tick"));
            alien.AnimationComplete += (sender, args) => sounds["tick"].Play();
            Direction = 1;
            Speed = 1;
        }
        #endregion

        #region functions
        /// <summary>
        /// Draw this gameobject's active sprite
        /// </summary>
        /// <param name="batch"></param>
        public override void Draw(SpriteBatch batch)
        {
            Sprite.Draw(batch);
        }

        /// <summary>
        /// redundant method call for this gameobject since it is non interactive.
        /// The other option is to use multiple interfaces with casts all over the place
        /// </summary>
        public override void OnInput(GameTime gt)
        {
        }

        /// <summary>
        /// Logic for updating position and non-graphics related changes. These changes to the UI will be reflected in the Draw call
        /// </summary>
        /// <param name="gt"></param>
        public override void Update(GameTime gt)
        {
            Sprite.Position.X = (alien.Position.X + (Speed * Direction));
            Sprite.Update(gt);
        }

        /// <summary>
        /// This object is destroyed and added to the RemoveQueue for removal in the next Update loop in the game
        /// </summary>
        public override void Destroy()
        {
            Goto(State.Dead);
            explosion.Hide();
            removeQueue.Enqueue(new Tuple<int, GameObject>(Id, this));
            bag.Remove(Id);
        }

        /// <summary>
        /// Method called when this alien collides with another object (bullet, commander, bunker, etc)
        /// </summary>
        /// <param name="otherObject"></param>
        public override void CollidedWith(GameObject otherObject)
        {
            Goto(State.Dying);
            currentSprite = explosion;
            explosion.Position = alien.Position;
            alien.Hide();
            explosion.Show();
        }

        /// <summary>
        /// Transition to the next state and update the 'Current' variable with the new state
        /// </summary>
        /// <param name="next"></param>
        public void Goto(State next)
        {
            current = next;
        }
        #endregion
    }
}
