using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src;
using NetworkIO.src.controllers;
using NetworkIO.src.menu;
using System.Collections.Generic;

namespace NetworkIO
{
    public class Background : Controller
    {
        protected float relativeSpeed; //1->0
        protected Camera camera;
        public Background(List<src.IControllable> collidables, float relativeSpeed, Camera camera) : base(collidables)
        {
            this.relativeSpeed = relativeSpeed;
            this.camera = camera;
        }
        
        public override void Update(GameTime gameTime) //OBS: assumes background sprites not rotated
        {
            foreach (WorldEntity e in controllables)
            {
                Vector2 cameraChange = camera.Position - camera.PreviousPosition;
                Vector2 positionChange = cameraChange* (1 - relativeSpeed);
                e.Position += positionChange;
                e.TotalExteriorForce *= (1-relativeSpeed);
                e.Update(gameTime);
                UpdatePosition();
                UpdateRadius();
            }
        }
    }
}