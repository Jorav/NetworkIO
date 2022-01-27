using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.collidables
{
    class CollidableCircle : ICollidable
    {
        public Vector2 Position { set; get; }
        public float Radius { set; get; }

        public CollidableCircle(Vector2 position, float radius)
        {
            Position = position;
            Radius = radius;
        }

        public bool collidesWith(ICollidable c)
        {
            if (c is CollidableCircle)
                return collidesWithCircle((CollidableCircle) c);
            //TODO: implementera för rektabgel
            throw new NotImplementedException();
        }
        
        //TODO: dubbelkolla att detta stämmer
        public bool collidesWithCircle(CollidableCircle c)
        {
            return Math.Sqrt(Math.Pow((double)(Position.X) - (double)(c.Position.X), 2) + Math.Pow((double)(Position.Y) - (double)(c.Position.Y), 2)) <= (Radius + c.Radius);
        }
    }
}
