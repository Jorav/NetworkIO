using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class CircularAI : CollidablesController
    {
        public CircularAI(List<ICollidable> collidables) : base(collidables)
        {
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
                    Vector2 accelerationVector = new Vector2((float)Math.Cos(e.Rotation+=0.02f), (float) Math.Sin(e.Rotation += 0.02f));
                    accelerationVector.Normalize();
                    e.Accelerate(accelerationVector, e.Thrust);
            }
        }
    }
}
