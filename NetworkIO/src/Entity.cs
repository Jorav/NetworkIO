using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.collidables;
using NetworkIO.src.entities;
using NetworkIO.src.factories;
using System;

namespace NetworkIO.src
{
    public class Entity : Movable
    {
        public float Elasticity;
        public bool IsVisible { get; set; }
        public bool IsCollidable { get; set; }

        protected Sprite sprite = null;
        private CollidableRectangle collisionDetector;

        public override Vector2 Position { get { return position; } set{ position = value; sprite.Position = value; collisionDetector.Position = value; } }
        public override float Rotation { get { return rotation; } set { rotation = value; sprite.Rotation = value; collisionDetector.Rotation = value; } }
        public Vector2 Origin { get { return origin; } set { origin = value; sprite.Origin = value; collisionDetector.Origin = value; } }
        protected Vector2 origin;
        public float Width { get { return sprite.Width; } }
        public float Height { get { return sprite.Height; } }
        public float Health { get { return health; } set { health = value; if(value<=0) Die(); } }
        protected float health;

        public Entity(Sprite sprite, Vector2 position, float rotation = 0, float mass = 1, float thrust = 1, float friction = 0.1f, float health = 100, bool isVisible = true, bool isCollidable = true,  float elasticity = 1) : base(position, rotation, mass, thrust, friction)
        {
            this.sprite = sprite;
            collisionDetector = (CollidableRectangle) CollidableFactory.CreateCollissionDetector(position, rotation, sprite.Width, sprite.Height);
            Elasticity = elasticity;
            Health = health;
            IsVisible = isVisible;
            IsCollidable = isCollidable;
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

        public override void Update(GameTime gameTime) //OBS Ska vara en funktion i thruster
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
                float r = Vector2.Distance(Position, e.Position);
                Vector2 directionalVector = Position - e.Position;
                directionalVector.Normalize();
                TotalExteriorForce += Physics.CalculateCollisionRepulsion(Position, Velocity, Mass, e.Position, e.Velocity, e.Mass, Math.Min(Math.Max(e.Width, e.Height), Math.Max(Width, Height)), Elasticity, e.Elasticity); //OBS: might break after changes
            }
        }

        public override object Clone()
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
