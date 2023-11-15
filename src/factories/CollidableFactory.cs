using Microsoft.Xna.Framework;
using NetworkIO.src.collidables;
using NetworkIO.src.collission_detectors;
using NetworkIO.src.utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.factories
{
    public static class CollidableFactory
    {
        public static CollisionDetector CreateRectangular(Vector2 position, float rotation, int width, int height)
        {
                //return new CollidableCircle(position, rotation);
            return new CollidableRectangle(position, rotation, width, height);
        }
        public static CollisionDetector CreateCircular(Vector2 position, float radius)
        {
            return new CollidableCircle(position, radius);
        }
    }
}
