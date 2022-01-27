using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.entities
{
    class Projectile : Entity
    {
        private float timer;
        private float lifeSpan;

        public Projectile(Sprite sprite, Vector2 position, float rotation, float mass, float thrust, float timer, float lifeSpan, bool isVisible = false, float friction = 0f) : base(sprite, position, rotation, mass, thrust, isVisible, friction)
        {
            this.timer = timer;
            this.lifeSpan = lifeSpan;
        }
        public override void Move(GameTime gameTime)
        {
            if (IsVisible)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (timer > lifeSpan)
                {
                    IsVisible = false;
                    timer = 0;
                    TotalExteriorForce = Vector2.Zero;
                    velocity = Vector2.Zero;
                }
            }
            base.Move(gameTime);
        }
        public override object Clone()
        {
            Projectile pNew = (Projectile)base.Clone();
            pNew.timer = 0;
            return pNew;
        }
    }
}
