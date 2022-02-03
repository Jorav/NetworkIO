using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    static class Physics
    {
        public static float gravityConstant = 0.4f;
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
        public static float CalculateRepulsion(float potentialThrust, float distance, float size)
        {
            if (distance < 10)
                distance = 10;
            return (float)(gravityConstant*potentialThrust / Math.Pow((double)distance/size, 2));//TODO: make sure no divide by 0
        }

        public static Vector2 CalculateCollisionBounce(Entity e1, Entity e2)
        {
            float distance = (e1.Position - e2.Position).Length();
            Vector2 directionalVector = e1.Position - e2.Position;

            //float partOfMomentum = e1.MomentumAlongVector(directionalVector).Length() / e1.MomentumAlongVector(directionalVector).Length() + e2.MomentumAlongVector(directionalVector).Length();
            //e1.MomentumAlongVector(directionalVector) + e2.MomentumAlongVector(directionalVector);
            return e2.MomentumAlongVector(directionalVector);
            //return (float)(gravityConstant / Math.Pow((double)distance/ Math.Min(e1.Width, e1.Height), 2));//TODO: make sure no divide by 0
            //return e1.Velocity - 2 * e2.Mass / (e1.Mass + e2.Mass) * Vector2.Dot(e1.Velocity - e2.Velocity, directionalVector) / (float)Math.Pow(directionalVector.Length(), 2) * directionalVector;

        }

        public static float CalculateAttraction(float attractionForce1, float attractionForce2, float distance)
        {
            if (distance < 10)//TODO: make this depend on velocity
                distance = 10;
            return (float)(attractionForce1 * attractionForce2 / Math.Pow(distance, 1) * gravityConstant);
        }
    }
}
