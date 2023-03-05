using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.entities;
using NetworkIO.src.hitboxes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    public abstract class CommandController : Controller
    {


        public CommandController(List<Unit> units) : base(units)
        {
        }

        public override void Update(GameTime gameTime)
        {
            UpdateEntities(gameTime);
            ApplyInternalGravity();
            UpdatePosition();
            UpdateRadius();
        }

        //protected abstract void GiveOrders();

        protected void ApplyInternalGravity()
        {
            foreach (Unit u1 in units)
            {
                foreach (Unit u2 in units)
                {
                    if (u1.IsVisible && u2.IsVisible)
                    {
                        float r = Vector2.Distance(u1.Position, u2.Position);
                        if (u1 != u2)
                        {
                            if (r < 10)
                                r = 10;
                            float res = Physics.CalculateGravity(0.05f, 0.05f, 30f, 30f, r);
                            u1.Accelerate(Vector2.Normalize(u2.Position - u1.Position), res);
                        }
                    }
                }
            }
        }
    }
}
