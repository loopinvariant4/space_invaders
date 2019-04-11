using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI
{
    interface IPhysicsObject
    {
        event EventHandler<CollisionEventArgs> OnCollide;
    }


    public class CollisionEventArgs : EventArgs
    {
        public GameObject OtherObject { get; private set; }

        public CollisionEventArgs(GameObject otherObject)
        {
            OtherObject = otherObject;
        }
    }
}
