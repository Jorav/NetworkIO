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
            Accelerate();
            base.Update(gameTime);
        }
        protected void Rotate()
        {
            foreach (Entity e in entities)
                if(e.IsVisible)
                    e.RotateTo(enemy.entities[0].Position);
        }
        protected void Accelerate()
        {
            foreach (Entity e in entities)
            {
                    Vector2 accelerationVector = enemy.entities[0].Position + enemy.entities[0].Velocity - (e.Position + e.Velocity);
                    accelerationVector.Normalize();
                    e.Accelerate(accelerationVector, e.Thrust);
            }
        }
    }
}
