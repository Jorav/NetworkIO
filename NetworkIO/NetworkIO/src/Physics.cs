using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    static class Physics
    {
        public static float gravityConstant = 1000f;
        private static Vector2 FrictionForce(Vector2 velocity, Vector2 totalExteriorForce, float mass, float frictionPercent)
        {
            return (velocity + totalExteriorForce/mass) * frictionPercent;
        }
        private static Vector2 Acceleration(Vector2 velocity, Vector2 totalExteriorForce, float mass, float frictionPercent) {
            return (totalExteriorForce - FrictionForce(velocity, totalExteriorForce, mass, frictionPercent)) / mass;
        }
        public static Vector2 CalculateVelocity(Vector2 position, Vector2 velocity, Vector2 totalExteriorForce, float mass, float frictionPercent)
        {
                return velocity + Acceleration(velocity, totalExteriorForce, mass, frictionPercent);
        }
        public static float CalculateRepulsion(float momentum1, float momentum2, float distance)
        {
            if (distance < 10)//TODO: make this depend on velocity
                distance = 10;
            return (float)(gravityConstant / Math.Pow((double)distance, 2) * momentum2/(momentum1+momentum2));//TODO: make sure no divide by 0
        }

        public static float CalculateAttraction(float attractionForce1, float attractionForce2, float distance)
        {
            if (distance < 10)//TODO: make this depend on velocity
                distance = 10;
            return (float)(attractionForce1 * attractionForce2 / Math.Pow(distance, 1) * gravityConstant);
        }
    }
}
