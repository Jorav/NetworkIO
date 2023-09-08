using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.movable
{
    public abstract class Entity : Movable, IControllable
    {
        public float Elasticity;
        public bool IsVisible { get; set; }
        public bool IsCollidable { get; set; }
        public virtual float Radius { get; }

        public Entity(Vector2 position, float rotation = 0, float mass = 1, float thrust = 1, float friction = 0.1f, float elasticity = 1) : base(position, rotation, mass, thrust, friction) { Elasticity = elasticity; }

        public abstract bool Contains(Vector2 point);
        public abstract bool ContainsInSpace(Vector2 position, Matrix transform);
        public abstract bool CollidesWith(IIntersectable c);
        public abstract void Draw(SpriteBatch sb);
        public abstract void Collide(IControllable c);
        public virtual void Shoot(GameTime gameTime)
        {
        }
    }
}
