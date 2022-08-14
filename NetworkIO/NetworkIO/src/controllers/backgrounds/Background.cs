using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
                Vector2 cameraChange = camera.Position - camera.PreviousPosition;
                Vector2 positionChange = (1 - relativeSpeed) * cameraChange;
                e.Position += positionChange;
                e.TotalExteriorForce *= relativeSpeed;
                e.Move(gameTime);
                UpdatePosition();
                UpdateRadius();
            }
        }
        public override void Draw(SpriteBatch sb, Matrix parentMatrix)
        {
            /*
            parentMatrix *= Matrix.CreateScale(1/camera.Zoom/relativeSpeed, 1/camera.Zoom/relativeSpeed, 0);
            Matrix position = Matrix.CreateTranslation(
                -controller.Position.X,
                -controller.Position.Y,s
                0);
            Matrix offset = Matrix.CreateTranslation(
                Game1.ScreenWidth / 2,
                Game1.ScreenHeight / 2,
                0);
            Matrix rotation = Matrix.CreateRotationZ(Rotation);
            Matrix zoom = Matrix.CreateScale(Zoom, Zoom, 0);
            parentMatrix = position * rotation * zoom * offset;*/
            base.Draw(sb, parentMatrix);
        }

    }
}