using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class RandomAI : Controller
    {
        Random r;
        public RandomAI(List<Entity> entities) : base(entities)
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
            foreach (Entity e in entities)
            {
                    Vector2 accelerationVector = new Vector2((float)Math.Cos(angle), (float) Math.Sin(angle));
                    accelerationVector.Normalize();
                    e.Accelerate(accelerationVector, e.Thrust);
            }
        }
    }
}
