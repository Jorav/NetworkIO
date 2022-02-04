using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    static class Physics
    {
        public static float gravityConstant = 1f;
        public static float elasticCollisionLoss = 0.001f;
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
        public static Vector2 CalculateCollisionRepulsion(Vector2 position, Vector2 velocity, float mass, Vector2 positionOther, Vector2 velocityOther, float massOther, float size, float elasticityE, float repulsionForceOther)
        {
                return CalculateBounceForce(position, velocity, mass, positionOther, velocityOther, massOther)*elasticityE 
                + Vector2.Normalize(position - positionOther)* CalculateRepulsion(0.5f, repulsionForceOther, Vector2.Distance(position, positionOther), size); //TODO: 1,1 as input should be changed to scale/repulsionforce
        }
        private static Vector2 CalculateBounceForce(Vector2 position, Vector2 velocity, float mass, Vector2 positionOther, Vector2 velocityOther, float massOther)
        {
            if (!(Vector2.Dot(velocity, Vector2.Normalize(position - positionOther)) >= 0 && Vector2.Dot(velocityOther, Vector2.Normalize(positionOther - position)) >= 0))
                return mass * elasticCollisionLoss * velocity.Length() * (velocity - 2 * massOther / (mass + massOther)
                * Vector2.Dot(velocity - velocityOther, position/32 - positionOther/32)
                / (position/32 - positionOther/32).LengthSquared() * (position - positionOther));//OBSOBSOBS IDK why this works but it works
            else
                return Vector2.Zero;

        }
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
        }

        public static float CalculateGravity(float attractionForce1, float attractionForce2, float repulsionForce1, float repulsionForce2, float distance)
        {
            return gravityConstant*(CalculateAttraction(attractionForce1, attractionForce2, distance) - CalculateRepulsion(repulsionForce1, repulsionForce2, distance));
        }
    }
}
