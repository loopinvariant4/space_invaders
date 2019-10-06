using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI
{
    public abstract class GameStage
    {
        public string Id { get; set; }

        public string Next { get; set; }

        public string Previous { get; set; }

        public GameStage(string id)
        {
            Id = id;
        }

        
    }
}
