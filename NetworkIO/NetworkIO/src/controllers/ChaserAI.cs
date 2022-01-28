using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    class ChaserAI : Controller
    {
        private Controller enemy;
        public ChaserAI(List<Entity> entities, Controller enemy) : base(entities) //TODO: Change enemy targeting to something smarter
        {
            this.enemy = enemy;
        }
        public override void Update(GameTime gameTime)
        {
            Rotate();
            Move();
            base.Update(gameTime);
        }
        protected void Rotate()
        {
            foreach (Entity e in entities)
                e.RotateTo(enemy.entities[0].Position);
        }
        private void Move()
        {
            foreach (Entity e in entities)
            {
                Vector2 accelerationVector = enemy.entities[0].Position + enemy.entities[0].Velocity-(entities[0].Position+ entities[0].Velocity);
                accelerationVector.Normalize();
                e.Accelerate(accelerationVector, e.Thrust);
            }
        }
    }
}
