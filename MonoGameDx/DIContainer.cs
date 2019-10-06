using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI
{
    public static class DIContainer
    {
        private static Dictionary<string, object> items = new Dictionary<string, object>();

        public static void Add<T>(string id, T item)
        {
            items.Add(id, item);
        }

        public static T Get<T>(string id)
        {
            return (T)items[id];
        }

        public static bool Contains<T>(string id)
        {
            return items.ContainsKey(id);
        }

        public static void Clear()
        {
            items.Clear();
        }
    }
}
