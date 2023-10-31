using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.entities
{
    class Projectile : WorldEntity
    {
        private float timer = 0;
        public float MaxLifeSpan;
        public float MinLifeSpan;
        private float lowerVelocityLimit=3;

        public Projectile(Sprite sprite, Vector2 position, float maxLifeSpan = 3f, float minLifeSpan = 1f) : base(sprite, position, mass:0.4f, friction:0.03f, isVisible:false, isCollidable:false)
        {
            this.MaxLifeSpan = maxLifeSpan;
            this.MinLifeSpan = minLifeSpan;
        }
        public void Collide(WorldEntity e)
        {
            if (CollidesWith(e) && e.Team != Team)
            {
                e.Manager.Collide(this);
                base.Collide(e.Manager);
                e.Health--;
                IsCollidable = false;
            }
        }

        public override void Update(GameTime gameTime) //OBSOIBSOBSOBSSOBSBOSBOS This needs to happen when dying as well
        {
            if (IsAlive)
            {
                base.Update(gameTime);
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(timer > MinLifeSpan)
                    if (timer > MaxLifeSpan || Velocity.Length()<= lowerVelocityLimit)
                        Die();
                RotateTo(Position + Velocity);
            }
            
        }
        public override void Die()
        {
            timer = 0;
            health = 0;
            base.Die();
        }
        public override object Clone()
        {
            Projectile pNew = (Projectile)base.Clone();
            pNew.timer = 0;
            return pNew;
        }
    }
}
