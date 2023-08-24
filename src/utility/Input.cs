using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    public class Input
    {
        public Camera Camera { get; set; }
        public Keys Up { get; set; }
        public Keys Down { get; set; }
        public Keys Left { get; set; }
        public Keys Right { get; set; }
        public Keys Pause { get; set; }
        public Keys Build { get; set; }
        public Vector2 MousePositionGameCoords { get { return (Mouse.GetState().Position.ToVector2() / Camera.Zoom - new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2) / Camera.Zoom + Camera.Position); } }
        private bool pauseDown;
        public bool PauseClicked
        {
            get
            {
                bool pauseClicked = false;
                bool newPauseDown = Keyboard.GetState().IsKeyDown(Pause);
                if (!pauseDown && newPauseDown)
                {
                    pauseClicked = true;
                }
                pauseDown = newPauseDown;
                return pauseClicked;
            }
        }
        private bool buildDown;
        public bool BuildClicked
        {
            get
            {
                bool buildClicked = false;
                bool newBuildDown = Keyboard.GetState().IsKeyDown(Build);
                if (!buildDown && newBuildDown)
                {
                    buildClicked = true;
                }
                buildDown = newBuildDown;
                return buildClicked;
            }
        }
    }
}
