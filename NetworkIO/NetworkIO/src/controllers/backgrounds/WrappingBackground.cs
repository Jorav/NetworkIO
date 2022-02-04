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
                if (positionX + e.Width < camera.Position.X)
                    positionX += camera.Width + e.Width;
                else if(positionX > camera.Position.X+camera.Width)
                    positionX -= camera.Width+e.Width;
                if (positionY + e.Height < camera.Position.Y)
                    positionY += camera.Height + e.Height;
                else if (positionY > camera.Position.Y + camera.Height)
                    positionY -= camera.Height+e.Height;
                if (positionX != e.Position.X || positionY != e.Position.Y)
                    e.Position = new Vector2(positionX, positionY);
            }
        }


    }
}