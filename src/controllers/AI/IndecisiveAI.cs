using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class IndecisiveAI : CollidablesController
    {
        Random r;
        public IndecisiveAI(List<ICollidable> entities) : base(entities)
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
            foreach (Entity e in collidables)
            {
                    Vector2 accelerationVector = new Vector2((float)Math.Cos(r.NextDouble()*Math.PI*2), (float) Math.Sin(r.NextDouble() * Math.PI * 2));
                    accelerationVector.Normalize();
                    e.Accelerate(accelerationVector, e.Thrust);
            }
        }
    }
}
