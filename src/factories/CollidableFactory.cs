using Microsoft.Xna.Framework;
using NetworkIO.src.collidables;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.factories
{
    static class CollidableFactory
    {
        public static IIntersectable CreateCollissionDetector(Vector2 position, float rotation, int width, int height, bool isCircle = false)
        {
            if (isCircle)
                return new CollidableCircle(position, rotation);
            else
                return new CollidableRectangle(position, rotation, width, height);
        }
    }
}
