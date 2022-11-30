using Microsoft.Xna.Framework;
using NetworkIO.src.controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.entities.hulls
{
    public class RectangularComposite : Composite
    {

        public RectangularComposite(Sprite sprite, Vector2 position, CompositeController controller = null) : base(sprite, position)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != null)
                {
                    components[i].Update(gameTime);
                    components[i].Rotation = rotation - i * (float)Math.PI / 2;
                    components[i].Position = Position +
                        new Vector2((float)Math.Cos(components[i].Rotation) * Width / 2, (float)Math.Sin(components[i].Rotation) * Height / 2) +
                        new Vector2((float)Math.Cos(components[i].Rotation) * components[i].Width / 2, (float)Math.Sin(components[i].Rotation) * components[i].Width / 2);
                }
            }
        }
    }
}
