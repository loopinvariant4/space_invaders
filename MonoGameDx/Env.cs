using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI
{
    public class Env
    {
        private static Viewport viewport;
        public static Viewport Screen
        {
            get
            {
                return viewport;
            }
            set
            {
                viewport = value;
            }
        }
    }
}
