using Microsoft.Xna.Framework;
using NetworkIO.src.controllers;
using NetworkIO.src.entities.hulls;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.entities.world_entities.hulls
{
    public class TriangularEqualLeggedComposite : Composite
    {
        public TriangularEqualLeggedComposite(Sprite sprite, Vector2 position, EntityController controller = null) : base(sprite, position)
        {
            //(CollidableCircle)CollidableFactory.CreateCollissionDetector(position, rotation, sprite.Width, sprite.Height);
        }
        protected override void AddLinks()
        {
            if (Links.Count > 0)
                Links.Clear();
            Links.Add(new Link(new Vector2(-Width / 2, 0), this));
            Links.Add(new Link(Links[0].relativePosition + new Vector2(Width/2, -Height / 2), this));
            Links.Add(new Link(Links[0].relativePosition + new Vector2(Width / 2, Height / 2), this));
        }
    }
}
