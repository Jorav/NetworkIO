using Microsoft.Xna.Framework;
using NetworkIO.src.collission_detectors;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.collidables
{
    public class CollidableCircle : CollisionDetector
    {
        public Vector2 Position {  get; set; }
        private float radius;
        public float Radius { get { return radius * scale; } set { radius = value / scale; } }
        private float scale;
        public float Scale { get { return scale; } set { scale = value; } }
        public float Rotation { get; set; }
        private CollidableRectangle stretchedRectangle;

        public CollidableCircle(Vector2 position, float radius)
        {
            Position = position;
            Radius = radius;
            scale = 1;
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
        public bool CollidesWithCircle(CollidableCircle c)
        {
            return Math.Sqrt(Math.Pow((double)(Position.X) - (double)(c.Position.X), 2) + Math.Pow((double)(Position.Y) - (double)(c.Position.Y), 2)) <= (Radius + c.Radius);
        }

        public void Collide(IIntersectable c) //TEMPORARY, THESE SHOULD NOT COLLIDE DIRECTLY (or be part of ICollide interface)
        {
            throw new NotImplementedException();
        }
        public bool CollidesWithStretch(CollisionDetector cd)
        {
            if (cd != null)
            {
                return stretchedRectangle.CollidesWith(cd);
            }
            return false;
        }
        public void StretchTo(CollisionDetector cd)
        {
            if(cd is CollidableCircle c)
            {
                Vector2 change = c.Position - Position;
                Vector2 perpendicular = new Vector2(change.Y, -change.X);
                perpendicular.Normalize();
                float angle = (float)(Math.Atan(change.Y/change.X));
                Vector2 UL = Position + perpendicular*Radius;
                Vector2 DL = Position - perpendicular*Radius;
                Vector2 UR = c.Position + perpendicular*c.Radius;
                Vector2 DR = c.Position - perpendicular*c.Radius;
                stretchedRectangle = new CollidableRectangle(UR, DL, DR, UR);
            }
        }

        public void StopStretch()
        {
            stretchedRectangle = null;
        }

        public bool Contains(Vector2 position)
        {
            return (position - Position).Length() <= Radius;
        }

        public bool ContainsInSpace(Vector2 positionInM, Matrix m)
        {
            Vector3 s,t;
            Quaternion q;
            m.Decompose(out s, out q, out t);
            float scale = (float)(Math.Sqrt(Math.Pow(s.X, 2) + Math.Pow(s.Y, 2)));
            return (Vector2.Transform(Position, m) - positionInM).Length() > Radius / scale;
        }
    }
}
