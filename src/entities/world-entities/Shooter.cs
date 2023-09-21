using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.entities
{
    class Shooter : WorldEntity
    {
        public float FireRatePerSecond { get; set; }
        public float FiringStrength { get; set; }
        float lastTimeFired;
        //TODO: add accuracy
        public Queue<Projectile> Projectiles { get; set; }
        //Controller
        public Shooter(Sprite sprite, Vector2 position, Projectile projectile, float fireRatePerSecond = 15f, float firingStrength = 10f) : base(sprite, position)
        {
            this.FireRatePerSecond = fireRatePerSecond;
            this.FiringStrength = firingStrength;
            Projectiles = new Queue<Projectile>();
            Projectiles.Enqueue(projectile);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (Projectile p in Projectiles)
                p.Update(gameTime);
        }

        public override void Shoot(GameTime gameTime)
        {
            float currentTime = (float)gameTime.TotalGameTime.TotalSeconds;
            if (currentTime-lastTimeFired > (1 / FireRatePerSecond))
            {
                Projectile p;
                
                if(!Projectiles.Peek().IsVisible)
                {
                    p = Projectiles.Dequeue();
                    
                    Projectiles.Enqueue(p);
                }

                else
                {
                    p = ((Projectile)Projectiles.Peek().Clone());
                    Projectiles.Enqueue(p);
                }
                p.IsVisible = true; //TODO: Change to collisiondamage - advanced momentum+shell system
                p.IsCollidable = true;
                p.Position = Position;
                p.Rotation = Rotation;
                Vector2 directionalVector = new Vector2((float)Math.Cos(p.Rotation), (float)Math.Sin(p.Rotation));
                p.Velocity = EntityController.Velocity; //give velocity to projectile corresponding to shooter movement
                p.Accelerate(p.Rotation, FiringStrength);
                lastTimeFired = currentTime;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            foreach (Projectile p in Projectiles)
                p.Draw(sb);
            base.Draw(sb);
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
