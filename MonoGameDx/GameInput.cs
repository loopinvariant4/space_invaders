using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI
{
    public class GameInput
    {
        private Keys[] keys;

        public Keys[] Keys
        {
            get
            {
                return keys;
            }
            set
            {
                PreviousKeys = keys;
                keys = value;
            }
        }

        public Keys[] PreviousKeys { get; private set; }

        public bool IsKeyDown(Keys key)
        {
            return Keys.Contains(key);
        }

        public bool IsKeyPress(Keys key)
        {
            return PreviousKeys != null && PreviousKeys.Contains(key) == true && IsKeyDown(key) == false;
        }

    }
}
