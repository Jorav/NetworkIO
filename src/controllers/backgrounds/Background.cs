using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src;
using NetworkIO.src.controllers;
using NetworkIO.src.menu;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NetworkIO
{
    public class Background : Controller
    {
        protected float relativeSpeed; //1->0
        protected Camera camera;
        protected Vector2 movement;
        public Background(List<src.IControllable> collidables, float relativeSpeed, Camera camera, [OptionalAttribute] Vector2 movement) : base(collidables)
        {
            this.relativeSpeed = relativeSpeed;
            this.camera = camera;
            if (movement == null)
                movement = Vector2.Zero;
            this.movement = movement;
        }
        
        public override void Update(GameTime gameTime) //OBS: assumes background sprites not rotated
        {
            foreach (WorldEntity e in controllables)
            {
                Vector2 cameraChange = camera.Position - camera.PreviousPosition;
                Vector2 positionChange = cameraChange* (1 - relativeSpeed) + movement*relativeSpeed;
                //e.Accelerate(movement * relativeSpeed);
                e.Position += positionChange;
                e.Scale = 1.5f-camera.Zoom*(relativeSpeed);
                e.TotalExteriorForce *= (1-relativeSpeed);
                e.Update(gameTime);
                UpdatePosition();
                UpdateRadius();
            }
        }
    }
}