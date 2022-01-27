using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NetworkIO.src.collidables;
using NetworkIO.src.factories;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    class Entity
    {
        public float Thrust;
        protected float mass;
        public bool IsVisible { get; set; }
        protected float turnSpeed; //TODO: Implement this?

        protected Sprite sprite;
        private CollidableRectangle collissionDetector;

        public Vector2 Position { get { return position; } set{ position = value; sprite.Position = value; collissionDetector.Position = value; } }
        protected Vector2 position; 
        public float Rotation { get { return rotation; } set { rotation = value; sprite.Rotation = value; collissionDetector.Rotation = value; } }
        protected float rotation;

        protected Vector2 velocity;
        public float Friction { get; set; } // percent, where 0.1f = 10% friction
        public Vector2 TotalExteriorForce;
        
        public Entity(Sprite sprite, Vector2 position, float rotation, float mass, float thrust, bool isVisible = true, float friction = 0.1f)
        {
            this.sprite = sprite;
            collissionDetector = (CollidableRectangle) CollidableFactory.CreateCollissionDetector(position, rotation, sprite.Width, sprite.Height);
            Position = position;
            Rotation = rotation;
            this.mass = mass;
            this.Thrust = thrust;
            Friction = friction;
            IsVisible = isVisible;
            velocity = Vector2.Zero;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            if (IsVisible)
            {
                sprite.Draw(sb);
            }
            
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
            velocity = Physics.CalculateVelocity(Position, velocity, TotalExteriorForce, mass, Friction);
            Position += velocity;
            TotalExteriorForce = Vector2.Zero;
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
            eNew.collissionDetector = new CollidableRectangle(Position, Rotation, eNew.collissionDetector.Width, eNew.collissionDetector.Height);
            eNew.velocity = Vector2.Zero;
            eNew.TotalExteriorForce = Vector2.Zero;
            return eNew;
        }
    }
}
