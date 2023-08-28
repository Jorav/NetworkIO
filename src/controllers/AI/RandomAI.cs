using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class RandomAI : CollidablesController
    {
        Random r;
        public RandomAI(List<ICollidable> collidables) : base(collidables)
        {
            r = new Random();

        }

        public override void Update(GameTime gameTime)
        {
            Accelerate();
            base.Update(gameTime);
        }

        protected void Accelerate()
        {
            double angle = r.NextDouble() * Math.PI * 2;
            foreach (Entity e in collidables)
            {
                    Vector2 accelerationVector = new Vector2((float)Math.Cos(angle), (float) Math.Sin(angle));
                    accelerationVector.Normalize();
                    e.Accelerate(accelerationVector, e.Thrust);
            }
        }
    }
}
