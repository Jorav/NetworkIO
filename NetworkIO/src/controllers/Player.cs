using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NetworkIO.src.entities;
using System;
using System.Collections.Generic;

namespace NetworkIO.src
{
    public class Player : Controller
    {
        public Input Input { get; set; }
        public Camera Camera { get; private set; }
        private bool pauseDown;
        public bool PauseClicked
        {
            get
            {
                bool pauseClicked = false;
                bool newPauseDown = Keyboard.GetState().IsKeyDown(Input.Pause);
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
                bool newBuildDown = Keyboard.GetState().IsKeyDown(Input.Build);
                if (!buildDown && newBuildDown)
                {
                    buildClicked = true;
                }
                buildDown = newBuildDown;
                return buildClicked;
            }
        }
        public Player(List<Entity> entities) : base(entities)
        {
            Input = new Input()
            {
                Up = Keys.W,
                Down = Keys.S,
                Left = Keys.A,
                Right = Keys.D,
                Pause = Keys.Escape,
                Build = Keys.Enter,
            };
            Camera = new Camera(this);
        }

        public override void Update(GameTime gameTime)
        {
            Rotate();
            Accelerate();
            Shoot(gameTime);
            Camera.Update();
            base.Update(gameTime);
            /*
             * Rotate, calculate course, check collisions, update course, move, base.update
             */
        }
        private void Shoot(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                foreach (Entity e in entities)
                    if(e is Shooter gun)
                        gun.Shoot(gameTime);
        }
        protected void Rotate()
        {
            foreach (Entity e in entities)
                e.RotateTo(Mouse.GetState().Position.ToVector2()-new Vector2(Game1.ScreenWidth/2, Game1.ScreenHeight/2) +Camera.Position);
        }
        protected void Accelerate() //TODO(lowprio): remove vector 2 instanciation from angle calculation (inefficient, high computational req)
        {
            if (Input == null)
                return;
            Vector2 accelerationVector = Vector2.Zero;
            if (Keyboard.GetState().IsKeyDown(Input.Up) ^ Keyboard.GetState().IsKeyDown(Input.Down))
            {
                if (Keyboard.GetState().IsKeyDown(Input.Up))
                {
                    //accelerationVector.X += (float)Math.Cos((double)MathHelper.ToRadians(90));
                    accelerationVector.Y += (float)Math.Sin((double)MathHelper.ToRadians(-90));
                }
                else if (Keyboard.GetState().IsKeyDown(Input.Down))
                {
                    //accelerationVector.X += (float)Math.Cos((double)MathHelper.ToRadians(270));
                    accelerationVector.Y += (float)Math.Sin((double)MathHelper.ToRadians(-270));
                }
            }
            if (Keyboard.GetState().IsKeyDown(Input.Left) ^ Keyboard.GetState().IsKeyDown(Input.Right))
            {
                if (Keyboard.GetState().IsKeyDown(Input.Left))
                {
                    accelerationVector.X += (float)Math.Cos((double)MathHelper.ToRadians(180));
                    //accelerationVector.Y += (float)Math.Sin((double)MathHelper.ToRadians(-180));
                }
                else if (Keyboard.GetState().IsKeyDown(Input.Right))
                {
                    accelerationVector.X += (float)Math.Cos(0);
                    //accelerationVector.Y += (float)Math.Sin(0);
                }
            }
            if (!accelerationVector.Equals(Vector2.Zero))
            {
                accelerationVector.Normalize();
                foreach (Entity e in entities)
                    e.Accelerate(accelerationVector, e.Thrust);
            }
        }
    }
}