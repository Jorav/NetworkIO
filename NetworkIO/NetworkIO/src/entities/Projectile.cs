﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.entities
{
    class Projectile : Entity
    {
        private float timer = 0;
        private float lifeSpan;

        public Projectile(Sprite sprite, Vector2 position, float rotation, float mass, float thrust, float health, float lifeSpan, bool isVisible = false, bool isCollidable = false, float friction = 0f, float attractionForce = 1f, float repulsionForce = 1f) : base(sprite, position, rotation, mass, thrust, health, isVisible, isCollidable, friction, attractionForce, repulsionForce)
        {
            this.lifeSpan = lifeSpan;
        }
        public override void Collide(Entity e)
        {
            if (IsCollidable)
            {
                e.Collide(this);
                base.Collide(e);
                e.Health--;
                IsCollidable = false;
            }
        }

        public override void Move(GameTime gameTime) //OBSOIBSOBSOBSSOBSBOSBOS This needs to happen when dying as well
        {
            if (IsVisible)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (timer > lifeSpan)
                    Die();
            }
            base.Move(gameTime);
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
