using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class IndecisiveAI : Controller
    {
        Random r;
        public IndecisiveAI(List<Entity> entities) : base(entities)
        {
            r = new Random();

        }

        public override void Update(GameTime gameTime)
        {
            Move();
            base.Update(gameTime);
        }

        private void Move()
        {
            foreach (Entity e in entities)
            {
                    Vector2 accelerationVector = new Vector2((float)Math.Cos(r.NextDouble()*Math.PI*2), (float) Math.Sin(r.NextDouble() * Math.PI * 2));
                    accelerationVector.Normalize();
                    e.Accelerate(accelerationVector, e.Thrust);
            }
        }
    }
}
