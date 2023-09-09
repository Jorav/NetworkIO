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
        private bool inBuildScreen;
        public bool InBuildScreen { get { return inBuildScreen; } 
            set {
                if(value)
                    Zoom = BuildMenuZoom;
                else
                    Zoom = GameZoom;
                inBuildScreen = value;
            } 
        }
        public float BuildMenuZoom { get { return (Game1.ScreenHeight) / (2 * controller.Radius + Game1.ScreenHeight / 8); } }
        public float GameZoom { get { return Game1.ScreenHeight / (Game1.ScreenHeight + 1 * controller.Radius); } }
        private CollidableRectangle frame;
        private IControllable controller;
        private float zoomSpeed;

        public Camera(IControllable controller, bool inBuildScreen = false, float zoomSpeed = 0.05f)
        {
            
            this.controller = controller;
            Position = controller.Position;
            PreviousPosition = Position;
            Rotation = 0;
            InBuildScreen = inBuildScreen;
            this.zoomSpeed = zoomSpeed;
            UpdateTransformMatrix();
        }

        public void Update()
        {
            PreviousPosition = Position;
            Position = controller.Position;
            if (InBuildScreen)
            {
                AdjustZoom(BuildMenuZoom);
            }
            else
            {
                AdjustZoom(GameZoom);
            }
                
            Rotation = 0;
            UpdateTransformMatrix();
        }
        private void AdjustZoom(float optimalZoom)
        {
            if (optimalZoom > Zoom)
            {
                if (optimalZoom - Zoom > zoomSpeed)
                    Zoom += zoomSpeed;
                else
                    Zoom = optimalZoom;
            }
            else if (optimalZoom < Zoom)
            {
                if (Zoom - optimalZoom > zoomSpeed)
                    Zoom -= zoomSpeed;
                else
                    Zoom = optimalZoom;
            }
                


        }

        public void UpdateTransformMatrix()
        {
            Matrix position = Matrix.CreateTranslation(
                -controller.Position.X,
                -controller.Position.Y,
                0);
            Matrix rotation = Matrix.CreateRotationZ(Rotation);
            Matrix origin = Matrix.CreateTranslation(
                Game1.ScreenWidth / 2,
                Game1.ScreenHeight / 2,
                0);
            Matrix zoom = Matrix.CreateScale(Zoom, Zoom, 0);
            Transform = position * rotation * zoom * origin;
        }

        public bool InFrame(Vector2 position)
        {
            return frame.Contains(position);
        }
    }
}
