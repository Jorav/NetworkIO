using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    class ChaserAI : CollidablesController
    {
        private Controller enemy;
        public ChaserAI(List<ICollidable> collidables, Controller enemy) : base(collidables) //TODO: Change enemy targeting to something smarter
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
            foreach (Entity e in collidables)
                if(e.IsVisible)
                    e.RotateTo(enemy.collidables[0].Position);
        }
        protected void Accelerate()
        {
            foreach (Entity e in collidables)
            {
                Vector2 accelerationVector;
                if(enemy.collidables[0] is Entity)
                    accelerationVector = enemy.collidables[0].Position + ((Entity)(enemy.collidables[0])).Velocity - (e.Position + e.Velocity);
                else
                    accelerationVector = enemy.collidables[0].Position - (e.Position + e.Velocity);

                accelerationVector.Normalize();
                e.Accelerate(accelerationVector, e.Thrust);
            }
        }
    }
}
