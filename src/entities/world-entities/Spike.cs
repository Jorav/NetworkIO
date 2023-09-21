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

        public void Collide(WorldEntity e)
        {
            e.Health -= 10;
            e.EntityController.Collide(this); ;
        }
    }
}
