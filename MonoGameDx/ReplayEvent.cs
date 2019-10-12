using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI
{
    public class ReplayEvent
    {
        public Int64 LoopId { get; private set; }
        public Keys[] Keys { get; private set; }

        public ReplayEvent(Int64 loopId, Keys[] keys)
        {
            LoopId = loopId;
            Keys = keys;
        }

        public override string ToString()
        {
            if (Keys.Length > 0)
            {
                return string.Concat(LoopId, ",", string.Join("|", Keys));
            }
            return "";
        }

        public static ReplayEvent GetEvent(string eventItem)
        {
            string[] tokens = eventItem.Split(",".ToCharArray());
            var loopId = Int64.Parse(tokens[0]);
            string[] cmds = tokens[1].Split("|".ToCharArray());
            var keys = cmds.Select<string, Keys>(s => (Keys)Enum.Parse(typeof(Keys), s)).ToArray();
            return new ReplayEvent(loopId, keys);
        }
    }
}
