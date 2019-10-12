using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI
{
    public abstract class GameObject
    {
        public GameObject()
        {
        }

        public abstract AnimatedSprite Sprite { get; }

        public int Id {get; set;}

        public abstract void Update(GameTime gt);

        public abstract void Draw(SpriteBatch batch);

        public abstract void OnInput(GameTime gt, GameInput input);

        public abstract void Destroy();

        public abstract void CollidedWith(GameObject otherObject);

        public abstract Rectangle Box { get; }

        public abstract State Current { get; }

    }
}
