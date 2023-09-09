using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.collidables
{
    public class CollidableCircle : IIntersectable
    {
        public Vector2 Position { set; get; }
        public float Radius { set; get; }

        public CollidableCircle(Vector2 position, float radius)
        {
            Position = position;
            Radius = radius;
        }

        public bool CollidesWith(IIntersectable c)
        {
            if (c is CollidableCircle cc)
                return CollidesWithCircle(cc);
            if (c is CollidableRectangle cr)
                return CollidesWithRectangle(cr);
            throw new NotImplementedException();
        }

        public bool CollidesWithRectangle(CollidableRectangle cr) //NOT TESTED
        {
            Vector2 unrotatedCircle = new Vector2(
                (float)(Math.Cos(cr.Rotation) * (Position.X - cr.Position.X) - Math.Sin(cr.Rotation) * (Position.Y - cr.Position.Y) + cr.Position.X),
                (float)(Math.Sin(cr.Rotation) * (Position.X - cr.Position.X) + Math.Cos(cr.Rotation) * (Position.Y - cr.Position.Y) + cr.Position.Y));
            float deltaX = unrotatedCircle.X - Math.Max(cr.Position.X-cr.Width/2, Math.Min(unrotatedCircle.X, cr.Position.X + cr.Width/2));
            float deltaY = unrotatedCircle.Y - Math.Max(cr.Position.Y, Math.Min(unrotatedCircle.Y - cr.Height/2, cr.Position.Y + cr.Height/2));
            return (deltaX * deltaX + deltaY * deltaY) <= (Radius * Radius);
        }

        //TODO: dubbelkolla att detta stämmer
        private bool CollidesWithCircle(CollidableCircle c)
        {
            return Math.Sqrt(Math.Pow((double)(Position.X) - (double)(c.Position.X), 2) + Math.Pow((double)(Position.Y) - (double)(c.Position.Y), 2)) <= (Radius + c.Radius);
        }

        public void Collide(IIntersectable c) //TEMPORARY, THESE SHOULD NOT COLLIDE DIRECTLY (or be part of ICollide interface)
        {
            throw new NotImplementedException();
        }
    }
}
