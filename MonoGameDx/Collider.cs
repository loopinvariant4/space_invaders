using System;
using System.Collections.Generic;

namespace SI
{
    public class Collider
    {
        private List<Tuple<GameObject, GameObject>> twoObjects = new List<Tuple<GameObject, GameObject>>();
        private List<Tuple<GameObject, GameObject[]>> oneToMany = new List<Tuple<GameObject, GameObject[]>>();
        private Queue<GameObject> itemsToRemove = new Queue<GameObject>();
        public void Register(GameObject obj1, GameObject obj2)
        {
            twoObjects.Add(new Tuple<GameObject, GameObject>(obj1, obj2));
        }

        public void Register(GameObject obj, GameObject[] objs)
        {
            oneToMany.Add(new Tuple<GameObject, GameObject[]>(obj, objs));
        }

        public void UnRegister(GameObject obj)
        {
            itemsToRemove.Enqueue(obj);
        }

        public void RemoveQueuedItems()
        {
            while (itemsToRemove.Count > 0)
            {
                GameObject obj = itemsToRemove.Dequeue();
                twoObjects.RemoveAll(t => t.Item1.Id == obj.Id);
                oneToMany.RemoveAll(t => t.Item1.Id == obj.Id);
            }

        }

        public void CheckCollisions()
        {
            foreach (Tuple<GameObject, GameObject> item in twoObjects)
            {
                GameObject obj1 = item.Item1;
                GameObject obj2 = item.Item2;

                if (obj1.Current == State.Alive && obj2.Current == State.Alive)
                {
                    CheckTwoObjects(obj1, obj2);
                }
            }

            foreach (Tuple<GameObject, GameObject[]> item in oneToMany)
            {
                GameObject obj1 = item.Item1;
                GameObject[] listobjs = item.Item2;

                foreach (GameObject obj2 in listobjs)
                {
                    if (obj1.Current != State.Alive) break;
                    if (obj2.Current != State.Alive) continue;
                    CheckTwoObjects(obj1, obj2);
                }
            }
        }

        private void CheckTwoObjects(GameObject obj1, GameObject obj2)
        {
            if (obj1.Sprite.Box.Intersects(obj2.Sprite.Box))
            {
                obj1.CollidedWith(obj2);
                obj2.CollidedWith(obj1);
            }
        }
    }
}
