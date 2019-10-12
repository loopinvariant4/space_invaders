using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI
{
    public class GameStageSettings
    {
        private Dictionary<string, object> settings = new Dictionary<string, object>();

        public object this[string key]
        {
            get
            {
                object o = null;
                return settings.TryGetValue(key, out o) ? o : null;
                
            }
        }

        public void Set(string key, object obj)
        {
            if(settings.ContainsKey(key))
            {
                settings.Remove(key);
            }
            settings.Add(key, obj);
        }
    }
}
