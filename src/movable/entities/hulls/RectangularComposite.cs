using Microsoft.Xna.Framework;
using NetworkIO.src.controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.entities.hulls
{
    public class RectangularComposite : Composite
    {

        public RectangularComposite(Sprite sprite, Vector2 position, EntityController controller = null) : base(sprite, position)
        {
        }

        protected override void AddLinks()
        {
            if (Links.Count > 0)
                Links.Clear();
            Links.Add(new Link(new Vector2(-Width / 2, 0), this));
            Links.Add(new Link(new Vector2(0, -Height/2), this));
            Links.Add(new Link(new Vector2(Width / 2, 0), this));
            Links.Add(new Link(new Vector2(0, Height / 2), this));
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            /*for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != null)
                {
                    components[i].Update(gameTime);
                    components[i].Rotation = rotation - i * (float)Math.PI / 2;
                    components[i].Position = Position +
                        new Vector2((float)Math.Cos(components[i].Rotation) * Width / 2, (float)Math.Sin(components[i].Rotation) * Height / 2) +
                        new Vector2((float)Math.Cos(components[i].Rotation) * components[i].Width / 2, (float)Math.Sin(components[i].Rotation) * components[i].Width / 2);
                }
            }*/
        }
    }
}
