using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    static class Physics
    {
        public static Vector2 CalculateCollissionRepulsion(Vector2 position, Vector2 positionOther, Vector2 velocity, Vector2 velocityOther)
        {
            Vector2 vectorFromOther = positionOther - position;
            float distance = vectorFromOther.Length();
            vectorFromOther.Normalize();
            return 0.5f*Vector2.Normalize(-vectorFromOther) * (Vector2.Dot(velocity, vectorFromOther) + Vector2.Dot(velocityOther, -vectorFromOther)); //make velocity depend on position
        }
        public static Vector2 CalculateOverlapRepulsion(Vector2 position, Vector2 positionOther, float radius, float scale = 1)
        {
            float distance = (position - positionOther).Length();
            if (distance < radius/2)
                distance = radius/2;
            return 1f*Vector2.Normalize(position - positionOther) / (float)Math.Pow(distance/radius / scale, 1/1);
        }
    }
}
