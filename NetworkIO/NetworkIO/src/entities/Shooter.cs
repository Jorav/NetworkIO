using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.entities
{
    class Shooter : Entity
    {
        float fireRatePerSecond;
        float lastTimeFired;
        float firingStrength;
        //TODO: add accuracy
        public Queue<Projectile> Projectiles { get; set; }
        //Controller
        public Shooter(Sprite sprite, Vector2 position, float rotation, float mass, float thrust, float health, float fireRatePerSecond, float firingStrength, Projectile projectile, bool isVisible = true, bool isCollidable = true, float friction = 0.1f, float attractionForce = 1f, float repulsionForce = 1f) : base(sprite, position, rotation, mass, thrust, health, isVisible, isCollidable, friction, attractionForce, repulsionForce)
        {
            this.fireRatePerSecond = fireRatePerSecond;
            this.firingStrength = firingStrength;
            Projectiles = new Queue<Projectile>();
            Projectiles.Enqueue(projectile);
        }

        public override void Move(GameTime gameTime)
        {
            base.Move(gameTime);
            foreach (Projectile p in Projectiles)
                p.Move(gameTime);
        }

        public void Shoot(GameTime gameTime)
        {
            float currentTime = (float)gameTime.TotalGameTime.TotalSeconds;
            if (currentTime-lastTimeFired > (1 / fireRatePerSecond))
            {
                Projectile p;
                if (Projectiles.Peek().IsVisible)
                {
                    p = ((Projectile)Projectiles.Peek().Clone());
                    p.IsCollidable = true;
                    Projectiles.Enqueue(p);
                }
                else
                {
                    p = Projectiles.Dequeue();
                    p.IsVisible = true;
                    p.IsCollidable = true;
                    Projectiles.Enqueue(p);
                }
                p.Position = Position + Velocity;
                p.Rotation = this.Rotation;
                Vector2 directionalVector = new Vector2((float)Math.Cos(p.Rotation), (float)Math.Sin(p.Rotation));
                p.Velocity = MomentumAlongVector(directionalVector); //give velocity to projectile corresponding to shooter movement
                p.Accelerate(p.Rotation, firingStrength);
                lastTimeFired = currentTime;
            }
        }

        public override void Draw(SpriteBatch sb, Matrix parentTransform)
        {
            foreach (Projectile p in Projectiles)
                p.Draw(sb, parentTransform);
            base.Draw(sb, parentTransform);
        }

        public override object Clone()
        {
            Shooter sNew = (Shooter)base.Clone();
            sNew.Projectiles = new Queue<Projectile>();
            sNew.Projectiles.Enqueue((Projectile)Projectiles.Peek().Clone());
            return sNew;
        }
    }
}
