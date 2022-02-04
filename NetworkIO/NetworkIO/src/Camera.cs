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
        public float Width { get { return Game1.ScreenWidth+controller.Radius; } }
        public float Height { get { return Game1.ScreenHeight+controller.Radius; } }

        private CollidableRectangle frame;
        private Controller controller;

        public Camera(Controller controller)
        {
            this.controller = controller;
            Position = controller.Position - new Vector2(Width / 2 - controller.Radius, Height / 2 - controller.Radius); //TODO: add linear velocity to camera;
            PreviousPosition = controller.Position;
            
        }

        public void Update()
        {
            PreviousPosition = Position;
            Position = controller.Position - new Vector2(Width / 2-controller.Radius, Height / 2 - controller.Radius); //TODO: add linear velocity to camera
            Matrix offset = Matrix.CreateTranslation(
                Width / 2,
                Height / 2,
                0);
            Matrix position = Matrix.CreateTranslation(
                -controller.Position.X - controller.Radius,
                -controller.Position.Y - controller.Radius,
                0);
            Transform = position * offset;
        }

        public bool InFrame(Vector2 position)
        {
            return frame.Contains(position);
        }
    }
}
