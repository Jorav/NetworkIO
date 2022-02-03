using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    public interface ICollidable
    {
        public bool CollidesWith(ICollidable c);
    }
}
