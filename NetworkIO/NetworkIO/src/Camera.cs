using Microsoft.Xna.Framework;
using NetworkIO.src.collidables;
using NetworkIO.src.factories;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    public class Camera
    {
        public Matrix Transform { get; private set; }
        public Vector2 Position { get; set; }
        public Vector2 PreviousPosition { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; set; }
        public float Width { get { return Game1.ScreenWidth / Zoom; } }
        public float Height { get { return Game1.ScreenHeight / Zoom; } }

        private CollidableRectangle frame;
        private Controller controller;

        public Camera(Controller controller)
        {
            this.controller = controller;
            Position = controller.Position;
            PreviousPosition = Position;
            Zoom = Game1.ScreenHeight / (Game1.ScreenHeight + 2 * controller.Radius);
            
            Rotation = 0;
        }

        public void Update()
        {
            PreviousPosition = Position;
            Position = controller.Position;
            Zoom = Game1.ScreenHeight / (Game1.ScreenHeight + 2 * controller.Radius);
            Rotation = 0;
            Matrix position = Matrix.CreateTranslation(
                -controller.Position.X,
                -controller.Position.Y,
                0);
            Matrix offset = Matrix.CreateTranslation(
                Game1.ScreenWidth / 2,
                Game1.ScreenHeight / 2,
                0);
            Matrix rotation = Matrix.CreateRotationZ(Rotation);
            Matrix zoom = Matrix.CreateScale(Zoom, Zoom, 0);
            Transform = position * rotation * zoom * offset;
        }

        public bool InFrame(Vector2 position)
        {
            return frame.Contains(position);
        }
    }
}
