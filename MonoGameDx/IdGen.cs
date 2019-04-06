using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public static class IdGen
    {
        private static int id = 1000;
        public static int Next
        {
            get
            {
                return id++;
            }
        }
    }
}
