using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.entities
{
    class Projectile : WorldEntity
    {
        private float timer = 0;
        private float maxLifeSpan;
        private float minLifeSpan;
        private float lowerVelocityLimit=2;

        public Projectile(Sprite sprite, Vector2 position, float maxLifeSpan = 20f, float minLifeSpan = 2f) : base(sprite, position, mass:0.4f, friction:0.03f, isVisible:false, isCollidable:false)
        {
            this.maxLifeSpan = maxLifeSpan;
            this.minLifeSpan = minLifeSpan;
        }
        public void Collide(WorldEntity e)
        {
            if (CollidesWith(e))
            {
                e.EntityController.Collide(this);
                base.Collide(e.EntityController);
                e.Health--;
                IsCollidable = false;
            }
        }

        public override void Update(GameTime gameTime) //OBSOIBSOBSOBSSOBSBOSBOS This needs to happen when dying as well
        {
            if (IsVisible)
            {
                base.Update(gameTime);
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(timer > minLifeSpan)
                    if (timer > maxLifeSpan || Velocity.Length()<= lowerVelocityLimit)
                        Die();
                RotateTo(Position + Velocity);
            }
            
        }
        public override void Die()
        {
            timer = 0;
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
