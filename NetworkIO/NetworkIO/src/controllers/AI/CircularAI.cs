using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class CircularAI : Controller
    {
        public CircularAI(List<Entity> entities) : base(entities)
        {
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
                    Vector2 accelerationVector = new Vector2((float)Math.Cos(e.Rotation+=0.02f), (float) Math.Sin(e.Rotation += 0.02f));
                    accelerationVector.Normalize();
                    e.Accelerate(accelerationVector, e.Thrust);
            }
        }
    }
}
