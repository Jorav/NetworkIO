using Microsoft.Xna.Framework;
using NetworkIO.src.controllers;
using NetworkIO.src.entities.hulls;
using NetworkIO.src.factories;
using System;

namespace NetworkIO.src.movable.entities.hull
{
    public class CircularComposite : Composite
    {
        public CircularComposite(Sprite sprite, Vector2 position, EntityController controller = null) : base(sprite, position)
        {
            //(CollidableCircle)CollidableFactory.CreateCollissionDetector(position, rotation, sprite.Width, sprite.Height);
        }

        /*protected override void CreateCollisionDetectors(Sprite sprite, Vector2 position, float rotation)
        {
            collisionDetector = CollidableFactory.CreateCircular(position, Math.Min(sprite.Width, sprite.Height));
            oldCollisionDetector = CollidableFactory.CreateCircular(position, Math.Min(sprite.Width, sprite.Height));
        }*/

        protected override void AddLinks()
        {
            if (Links.Count > 0)
                Links.Clear();
            Links.Add(new Link(new Vector2(-Width / 2, 0), this));
            Links.Add(new Link(new Vector2(0, -Height / 2), this));
            Links.Add(new Link(new Vector2(Width / 2, 0), this));
            Links.Add(new Link(new Vector2(0, Height / 2), this));
        }
    }
}