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

        public bool CollidesWithRectangle(CollidableRectangle r) //NOT TESTED
        {
            Vector2 unrotatedCircle = new Vector2(
                (float)(Math.Cos(r.Rotation) * (Position.X - r.Position.X) - Math.Sin(r.Rotation) * (Position.Y - r.Position.Y) + r.Position.X),
                (float)(Math.Sin(r.Rotation) * (Position.X - r.Position.X) + Math.Cos(r.Rotation) * (Position.Y - r.Position.Y) + r.Position.Y));
            
            float deltaX = unrotatedCircle.X - Math.Max(r.Position.X-r.Width/2, Math.Min(unrotatedCircle.X, r.Position.X + r.Width/2));
            float deltaY = unrotatedCircle.Y - Math.Max(r.Position.Y, Math.Min(unrotatedCircle.Y - r.Height/2, r.Position.Y + r.Height/2));
            return (deltaX * deltaX + deltaY * deltaY) <= (Radius * Radius);
            /*
            Vector2 closest;
            if (unrotatedCircle.X < r.Position.X)
                closest.X = r.Position.X;
            else if (unrotatedCircle.X > r.Position.X + r.Width)
                closest.X = r.Position.X + r.Width;
            else
                closest.X = unrotatedCircle.X;

            // Find the unrotated closest y point from center of unrotated circle
            if (unrotatedCircle.Y < r.Position.Y)
                closest.Y = r.Position.Y;
            else if (unrotatedCircle.Y > r.Position.Y + r.Height)
                closest.Y = r.Position.Y + r.Height;
            else
                closest.Y = unrotatedCircle.Y;

            // Determine collision
            bool collision = false;

            double distance = Vector2.Distance(unrotatedCircle, closest);
            if (distance < Radius)
                collision = true; // Collision
            else
                collision = false;
            return collision;*/
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
