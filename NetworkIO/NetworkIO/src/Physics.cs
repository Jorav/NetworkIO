using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    static class Physics
    {
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
    }
}
