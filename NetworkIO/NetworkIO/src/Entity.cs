using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.collidables;
using NetworkIO.src.entities;
using NetworkIO.src.factories;
using System;

namespace NetworkIO.src
{
    public class Entity
    {
        private static float bounceConstant = 0.4f;
        public float Thrust;
        public float Mass;
        public bool IsVisible { get; set; }
        public bool IsCollidable { get; set; }
        public float AttractionForce { get; set; }
        public float RepulsionForce { get; set; }

        protected float turnSpeed; //TODO: Implement this?

        protected Sprite sprite;
        private CollidableRectangle collisionDetector;

        public Vector2 Position { get { return position; } set{ position = value; sprite.Position = value; collisionDetector.Position = value; } }
        protected Vector2 position; 
        public float Rotation { get { return rotation; } set { rotation = value; sprite.Rotation = value; collisionDetector.Rotation = value; } }
        protected float rotation;
        public Vector2 Origin { get { return origin; } set { origin = value; sprite.Origin = value; collisionDetector.Origin = value; } }
        protected Vector2 origin;
        public float Width { get { return sprite.Width; } }
        public float Height { get { return sprite.Height; } }
        public float Health { get { return health; } set { health = value; if(value<=0) Die(); } }
        protected float health;

        public Vector2 Velocity { get; set; }
        public float Friction { get; set; } // percent, where 0.1f = 10% friction
        public Vector2 TotalExteriorForce;

        public Entity(Sprite sprite, Vector2 position, float rotation, float mass, float thrust, float health, bool isVisible = true, bool isCollidable = true, float friction = 0.1f, float attractionForce = 1f, float repulsionForce = 1f)
        {
            this.sprite = sprite;
            collisionDetector = (CollidableRectangle) CollidableFactory.CreateCollissionDetector(position, rotation, sprite.Width, sprite.Height);
            Position = position;
            Rotation = rotation;
            this.Mass = mass;
            Thrust = thrust;
            Health = health;
            Friction = friction;
            IsVisible = isVisible;
            IsCollidable = isCollidable;
            Velocity = Vector2.Zero;
            AttractionForce = attractionForce;
            RepulsionForce = repulsionForce;
            Origin = new Vector2(Width / 2, Height / 2);
        }

        public virtual void Draw(SpriteBatch sb, Matrix parentMatrix)
        {
            if (IsVisible)
            {
                sprite.Draw(sb, parentMatrix);
            }
            
        }
        public virtual void Die()
        {
            TotalExteriorForce = Vector2.Zero;
            Velocity = Vector2.Zero;
            IsVisible = false;
            IsCollidable = false;
        }

        /**
         * Accelerates a certain angle in radians
         */
        public void Accelerate(float angle, float thrust)
        {
            TotalExteriorForce += new Vector2((float)Math.Cos((double)angle),(float)Math.Sin((double)angle))*thrust;
        }

        /**
         * Recieved a normalized directional vector and accelerates with a certain thrust
         */
        public void Accelerate(Vector2 directionalVector, float thrust)
        {
            TotalExteriorForce += directionalVector * thrust;
        }
        public virtual void Move(GameTime gameTime) //OBS Ska vara en funktion i thruster
        {
            Velocity = Physics.CalculateVelocity(Position, Velocity, TotalExteriorForce, Mass, Friction);
            Position += Velocity;
            TotalExteriorForce = Vector2.Zero;
        }
        public bool CollidesWith(Entity e)
        {
            return IsCollidable && e.IsCollidable && collisionDetector.CollidesWith(e.collisionDetector);
        }
        public virtual void Collide(Entity e)
        {
            if (CollidesWith(e))
            {
                //TotalExteriorForce += Physics.CalculateCollisionBounce(this, e);
                
                float r = Vector2.Distance(Position, e.Position);
                Vector2 directionalVector = Position - e.Position;/*
                directionalVector.Normalize();
                Vector2 momentum = MomentumAlongVector(directionalVector);
                Vector2 momentumE = e.MomentumAlongVector(directionalVector);
                Vector2 momentumTotal = momentum + momentumE;
                float partOfMomentum = momentumE.Length() / (-momentum + momentumE).Length(); //(momentum.Length() + momentumE.Length());
                Accelerate(directionalVector, momentumTotal.Length()*partOfMomentum/(float)Math.Pow(Math.Max(Vector2.Distance(Position, e.Position), 10)/ (Width+Height)/2, 2)*0.1f);
                */
                Vector2 momentum = MomentumAlongVector(directionalVector);
                Vector2 momentumE = e.MomentumAlongVector(directionalVector);
                Vector2 momentumTotal = momentum + momentumE;
                directionalVector.Normalize();
                //float partOfMomentum = momentumE.Length() / (-momentum + momentumE).Length(); //(momentum.Length() + momentumE.Length());
                float partOfMomentum = momentumE.Length() / (momentum.Length() + momentumE.Length());
                if (partOfMomentum!=0)
                    Accelerate(Vector2.Normalize(Position - e.Position), Physics.CalculateRepulsion(Thrust, r, Math.Max(Width, Height)) * partOfMomentum + e.VelocityAlongVector(directionalVector).Length()*0.05f);
                if (!(e is Projectile))
                    Velocity = (bounceConstant) * Vector2.Normalize(Position - e.Position) * Velocity.Length(); //*/
            }
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
        /*public void RotateTo(Vector2 position)
        {
            Vector2 p = position - Position;
            if (p.X >= 0)
                Rotation = -(float)Math.Atan(p.Y / p.X);
            //else
                //Rotation = -(float)Math.Atan(p.Y / p.X) ;
        }*/
        public virtual object Clone()
        {
            Entity eNew = (Entity)this.MemberwiseClone();
            eNew.sprite = (Sprite)this.sprite.Clone();
            eNew.collisionDetector = new CollidableRectangle(position, rotation, (int)Width, (int)Height);
            eNew.Velocity = Vector2.Zero;
            eNew.TotalExteriorForce = Vector2.Zero;
            return eNew;
        }
    }
}
