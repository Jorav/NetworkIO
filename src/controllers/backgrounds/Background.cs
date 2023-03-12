using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src;
using NetworkIO.src.menu;
using System.Collections.Generic;

namespace NetworkIO
{
    public class Background : Controller
    {
        protected float relativeSpeed; //1->0
        protected Camera camera;
        public Background(List<Entity> entities, float relativeSpeed, Camera camera) : base(entities)
        {
            this.relativeSpeed = relativeSpeed;
            this.camera = camera;
        }
        
        public override void Update(GameTime gameTime) //OBS: assumes background sprites not rotated
        {
            foreach (Entity e in entities)
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