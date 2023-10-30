using Microsoft.Xna.Framework;
using NetworkIO.src.controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.movable.entities
{
    public class Spike : WorldEntity
    {
        public Spike(Sprite sprite, Vector2 position) : base(sprite, position)
        {
        }
        public override void HandleCollision(WorldEntity eOther, bool passesThroughFromBack = false, bool passesThroughFromFront = false)
        {
            base.HandleCollision(eOther, passesThroughFromBack, passesThroughFromFront);
            eOther.Health -= 1;
            Vector2 distance = (eOther.Position - Position);
            distance.Normalize();
            if (eOther.Manager != null && eOther.Manager is EntityController ec)
            {
                ec.Collide(this);
                ec.Accelerate(distance * 10);
            }
            if (Manager != null)
            {
                Manager.Accelerate(-distance * 10);
            }
        }
    }
}
