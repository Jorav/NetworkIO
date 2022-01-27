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
        Queue<Projectile> projectiles;
        public Shooter(Sprite sprite, Vector2 position, float rotation, float mass, float thrust, float fireRatePerSecond, float firingStrength, Projectile projectile, bool isVisible = true, float friction = 0.1f) : base(sprite, position, rotation, mass, thrust, isVisible, friction)
        {
            this.fireRatePerSecond = fireRatePerSecond;
            this.firingStrength = firingStrength;
            projectiles = new Queue<Projectile>();
            projectiles.Enqueue(projectile);
        }

        public override void Move(GameTime gameTime)
        {
            base.Move(gameTime);
            foreach (Projectile p in projectiles)
                p.Move(gameTime);
        }

        public void Shoot(GameTime gameTime)
        {
            float currentTime = (float)gameTime.TotalGameTime.TotalSeconds;
            if (currentTime-lastTimeFired > (1 / fireRatePerSecond))
            {
                Projectile p;
                if (projectiles.Peek().IsVisible)
                {
                    p = ((Projectile)projectiles.Peek().Clone());
                    projectiles.Enqueue(p);
                }
                else
                {
                    p = projectiles.Dequeue();
                    p.IsVisible = true;
                    projectiles.Enqueue(p);
                }
                p.Position = new Vector2(this.Position.X, this.Position.Y);
                p.Rotation = this.Rotation;
                p.Accelerate(p.Rotation, firingStrength);
                lastTimeFired = currentTime;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            foreach (Projectile p in projectiles)
                p.Draw(sb);
            base.Draw(sb);
        }

        public override object Clone()
        {
            Shooter sNew = (Shooter)base.Clone();
            sNew.projectiles = new Queue<Projectile>();
            sNew.projectiles.Enqueue((Projectile)projectiles.Peek().Clone());
            return sNew;
        }
    }
}
