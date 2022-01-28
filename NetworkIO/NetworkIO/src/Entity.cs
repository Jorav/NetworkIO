using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.collidables;
using NetworkIO.src.entities;
using NetworkIO.src.factories;
using System;

namespace NetworkIO.src
{
    class Entity
    {
        public float Thrust;
        protected float mass;
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
            this.mass = mass;
            Thrust = thrust;
            Health = health;
            Friction = friction;
            IsVisible = isVisible;
            IsCollidable = isCollidable;
            Velocity = Vector2.Zero;
            AttractionForce = attractionForce;
            RepulsionForce = repulsionForce;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            if (IsVisible)
            {
                sprite.Draw(sb);
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
        public virtual void Move(GameTime gameTime) //Ska vara en funktion i thruster
        {
            Velocity = Physics.CalculateVelocity(Position, Velocity, TotalExteriorForce, mass, Friction);
            Position += Velocity;
            TotalExteriorForce = Vector2.Zero;
        }
        public bool collidesWith(Entity e)
        {
            return IsCollidable && collisionDetector.collidesWith(e.collisionDetector);
        }
        public virtual void Collide(Entity e)
        {
            if (IsCollidable && e.IsCollidable)
            {
                float r = Vector2.Distance(Position, e.Position);
                Accelerate(Vector2.Normalize(Position - e.Position), Physics.CalculateRepulsion(Velocity.Length()*mass, e.Velocity.Length()*e.mass, r));
            }
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
            Entity eNew = (Entity)this.MemberwiseClone();
            eNew.sprite = (Sprite)this.sprite.Clone();
            eNew.collisionDetector = new CollidableRectangle(Position, Rotation, eNew.collisionDetector.Width, eNew.collisionDetector.Height);
            eNew.Velocity = Vector2.Zero;
            eNew.TotalExteriorForce = Vector2.Zero;
            return eNew;
        }
    }
}
