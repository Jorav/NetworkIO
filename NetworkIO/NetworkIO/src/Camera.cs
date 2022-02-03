using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    public class Camera
    {
        public Matrix Transform { get; private set; }
        public Vector2 Position { get; set; }

        public void Follow(Controller c)
        {
            Position = c.Position - new Vector2(Game1.ScreenWidth / 2-c.Radius, Game1.ScreenHeight / 2 - c.Radius);
            Matrix offset = Matrix.CreateTranslation(
                Game1.ScreenWidth / 2,
                Game1.ScreenHeight / 2,
                0);
            Matrix position = Matrix.CreateTranslation(
                -c.Position.X - c.Radius,
                -c.Position.Y - c.Radius,
                0);
            Transform = position * offset;
        }
    }
}
