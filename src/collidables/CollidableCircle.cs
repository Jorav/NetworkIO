using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.collidables
{
    public class CollidableCircle : ICollidable
    {
        public Vector2 Position { set; get; }
        public float Radius { set; get; }

        public CollidableCircle(Vector2 position, float radius)
        {
            Position = position;
            Radius = radius;
        }

        public bool CollidesWith(ICollidable c)
        {
            if (c is CollidableCircle cc)
                return CollidesWithCircle(cc);
            if (c is CollidableRectangle cr)
                return CollidesWithRectangle(cr);
            throw new NotImplementedException();
        }

        public bool CollidesWithRectangle(CollidableRectangle cr) //NOT TESTED
        {
            float deltaX = Position.X - Math.Max(cr.Position.X-cr.Width/2, Math.Min(Position.X, cr.Position.X + cr.Width/2));
            float deltaY = Position.Y - Math.Max(cr.Position.Y, Math.Min(Position.Y - cr.Height/2, cr.Position.Y + cr.Height/2));
            return (deltaX * deltaX + deltaY * deltaY) <= (Radius * Radius);
        }

        //TODO: dubbelkolla att detta stämmer
        private bool CollidesWithCircle(CollidableCircle c)
        {
            return Math.Sqrt(Math.Pow((double)(Position.X) - (double)(c.Position.X), 2) + Math.Pow((double)(Position.Y) - (double)(c.Position.Y), 2)) <= (Radius + c.Radius);
        }
    }
}
