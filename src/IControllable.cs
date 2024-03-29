﻿using Microsoft.Xna.Framework;
using NetworkIO.src.menu;
using NetworkIO.src.utility;
using System.Collections.Generic;

namespace NetworkIO.src
{
    public interface IControllable : IIntersectable, IComponent //should be renamed
    {
        public IController Manager {get; set;}
        public IDs Team { get; set; }
        public float Mass { get;}
        public Color Color { set; }
        public void Collide(IControllable c);
        public void RotateTo(Vector2 position);
        public void Accelerate(Vector2 directionalVector, float thrust);
        public void Accelerate(Vector2 directionalVector);
        public void Shoot(GameTime gameTime);
        public object Clone();
        /**
         * returns the sub-IControllable that contains 'position' in space 'transform', null if none does
         */
        public IControllable ControllableContainingInSpace(Vector2 position, Matrix transform);
        public void InteractWith(List<IControllable> controllers);
        //public void ApplyInternalRepulsion(IControllable controllable);
    }
}
