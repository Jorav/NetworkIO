using Microsoft.Xna.Framework;
using NetworkIO.src;
using System.Collections.Generic;

namespace NetworkIO
{
    public class Background : Controller
    {
        protected float relativeSpeed;
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
                if (e.Velocity.Length()<0.1f) {
                    Vector2 cameraChange = camera.Position - camera.PreviousPosition;
                    Vector2 positionChange = (1 - relativeSpeed) * cameraChange;
                    e.Position += positionChange;
                }
                e.Velocity *= relativeSpeed;
                e.Move(gameTime);
                UpdatePosition();
                UpdateRadius();
            }
        }


    }
}