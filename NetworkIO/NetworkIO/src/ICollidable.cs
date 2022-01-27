using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    interface ICollidable
    {
        public bool collidesWith(ICollidable c);
    }
}
