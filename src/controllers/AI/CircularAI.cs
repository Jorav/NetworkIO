using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class CircularAI : EntityController
    {
        public CircularAI(List<Entity> entities) : base(entities)
        {
        }

        public override void Update(GameTime gameTime)
        {
            Accelerate();
            base.Update(gameTime);
        }

        protected void Accelerate()
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
