using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    static class Physics
    {
        public static float elasticCollisionLoss = 0.0008f;
        private static Vector2 FrictionForce(Vector2 velocity, Vector2 totalExteriorForce, float mass, float frictionPercent)
        {
            return (velocity*mass + totalExteriorForce) * frictionPercent;
        }
        private static Vector2 Acceleration(Vector2 velocity, Vector2 totalExteriorForce, float mass, float frictionPercent) {
            return (totalExteriorForce - FrictionForce(velocity, totalExteriorForce, mass, frictionPercent)) / mass;
        }
        public static Vector2 CalculateVelocity(Vector2 position, Vector2 velocity, Vector2 totalExteriorForce, float mass, float frictionPercent)
        {
                return velocity + Acceleration(velocity, totalExteriorForce, mass, frictionPercent);
        }
        public static Vector2 CalculateBounceForce(Vector2 position, Vector2 velocity, float mass, Vector2 positionOther, Vector2 velocityOther, float massOther)
        {
            if (!(Vector2.Dot(velocity, Vector2.Normalize(position - positionOther)) > 0 && Vector2.Dot(velocityOther, Vector2.Normalize(positionOther - position)) > 0))
                return mass * elasticCollisionLoss * velocity.Length() * (velocity - 2 * massOther / (mass + massOther)
                * Vector2.Dot(velocity - velocityOther, position/32 - positionOther/32)
                / (position/32 - positionOther/32).LengthSquared() * (position - positionOther));//OBSOBSOBS IDK why this works but it works
            else
                return Vector2.Zero;

        }
        public static float CalculateAttraction(float radius1, float radius2, float distance, float scale = 1)
        {
            if (distance < 10)
                distance = 10;
            return (float)(1f*Math.Pow(distance/ (radius1 + radius2)/ scale, 1) );
        }
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
        public static float CalculateGravityRepulsion(float radius1, float radius2, float distance, float scale = 1)
        {
            if (distance < 10)
                distance = 10;
            return 0.1f / (float)Math.Pow(distance/ (2*Math.Max(radius1,radius2))  / scale, 2);
        }

        /*
        public static float CalculateAttraction(float attractionForce1, float attractionForce2, float distance, float scale = 1)
        {
            if (distance < 10)
                distance = 10;
            return (float)(attractionForce1 * attractionForce2 * Math.Pow(distance/scale, 1) );
        }
        public static float CalculateRepulsion(float repulsionForce1, float repulsionForce2, float distance, float scale = 1)
        {
            if (distance < 10)
                distance = 10;
            return repulsionForce1 * repulsionForce2 / (float)Math.Pow(distance/scale, 2);
        }*/
    }
}
