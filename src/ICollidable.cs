using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    public interface ICollidable
    {
        public Vector2 Position { get; set; }
        public bool CollidesWith(ICollidable c);
    }
}
