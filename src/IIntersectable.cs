using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    public interface IIntersectable
    {
        public Vector2 Position { get; set; }
        public float Radius { get; }
        public bool CollidesWith(IIntersectable c);
    }
}
