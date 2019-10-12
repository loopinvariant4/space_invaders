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
        public Keys[] Keys { get; set; }

        public bool IsKeyDown(Keys key)
        {
            return Keys.Contains(key);
        }

    }
}
