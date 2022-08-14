using Microsoft.Xna.Framework;
using NetworkIO.src;
using System.Collections.Generic;

namespace NetworkIO
{
    public class WrappingBackground : Background
    {
        public WrappingBackground(List<Entity> entities, float relativeSpeed, Camera camera) : base(entities, relativeSpeed, camera)
        {
        }

        public override void Update(GameTime gameTime) //OBS: assumes background sprites not rotated
        {
            base.Update(gameTime);
            foreach (Entity e in entities)
            {
                float positionX = e.Position.X;
                float positionY = e.Position.Y;
                if (positionX + e.Width/2*camera.Zoom <= camera.Position.X-camera.Width/2)
                    positionX += camera.Width + e.Width * camera.Zoom;//change this to setting new position instead of addition
                else if(positionX-e.Width/2 * camera.Zoom > camera.Position.X+camera.Width/2)
                    positionX -= camera.Width+e.Width * camera.Zoom;
                if (positionY + e.Height/2 * camera.Zoom <= camera.Position.Y-camera.Height/2)
                    positionY += camera.Height + e.Height * camera.Zoom;
                else if (positionY - e.Height/2 * camera.Zoom > camera.Position.Y + camera.Height/2)
                    positionY -= camera.Height+e.Height * camera.Zoom;
                if (positionX != e.Position.X || positionY != e.Position.Y)
                    e.Position = new Vector2(positionX, positionY);
            }
        }


    }
}