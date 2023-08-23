using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    public class Movable
    {
        protected float Mass { get; set; }
        public float Thrust { get; set; }
        // protected float turnSpeed; //TODO: Implement this?
        public virtual Vector2 Position { get; set; }
        protected Vector2 position;
        public virtual float Rotation { get; set; }
        protected float rotation;
        public Vector2 Velocity { get; set; }
        public float Friction { get; set; } // percent, where 0.1f = 10% friction
        public Vector2 TotalExteriorForce;

        public Movable(Vector2 position, float rotation = 0, float mass = 1, float thrust = 1, float friction = 0.1f)
        {
            this.position = position;
            this.rotation = rotation;
            Mass = mass;
            Thrust = thrust;
            Friction = friction;
            Velocity = Vector2.Zero;
        }

        public virtual void Update(GameTime gameTime) //OBS Ska vara en funktion i thruster
        {
            Velocity = Physics.CalculateVelocity(Position, Velocity, TotalExteriorForce, Mass, Friction);
            Position += Velocity;
            TotalExteriorForce = Vector2.Zero;
        }

        /**
         * Accelerates a certain angle in radians
         */
        public void Accelerate(float angle, float thrust) //TODO: Long term make "thruster" into its own entity type
        {
            TotalExteriorForce += new Vector2((float)Math.Cos((double)angle), (float)Math.Sin((double)angle)) * thrust;
        }

        /**
         * Recieved a directional vector and accelerates with a certain thrust
         */
        public void Accelerate(Vector2 directionalVector, float thrust)
        {
            Vector2 direction = new Vector2(directionalVector.X, directionalVector.Y);
            direction.Normalize();
            TotalExteriorForce += direction * thrust;
        }

        public Vector2 MomentumAlongVector(Vector2 directionalVector)
        {
            return VelocityAlongVector(directionalVector) * Mass;
        }

        public Vector2 VelocityAlongVector(Vector2 directionalVector)
        {
            directionalVector = new Vector2(directionalVector.X, directionalVector.Y);//unnecessary?
            directionalVector.Normalize();
            return Vector2.Dot(Velocity, directionalVector) / Vector2.Dot(directionalVector, directionalVector) * directionalVector;
        }

        public Vector2 Momentum()
        {
            return Velocity * Mass;
        }

        public void RotateTo(Vector2 position)
        {
            Vector2 p = position - Position;
            if (p.X >= 0)
                Rotation = (float)Math.Atan(p.Y / p.X);
            else
                Rotation = (float)Math.Atan(p.Y / p.X) - MathHelper.ToRadians(180);
        }

        public virtual object Clone()
        {
            Movable mNew = (Movable)this.MemberwiseClone();
            mNew.Velocity = Vector2.Zero;
            mNew.TotalExteriorForce = Vector2.Zero;
            return mNew;
        }
    }
}
