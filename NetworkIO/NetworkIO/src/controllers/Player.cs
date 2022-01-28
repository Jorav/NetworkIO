using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NetworkIO.src.entities;
using System;
using System.Collections.Generic;

namespace NetworkIO.src
{
    class Player : Controller
    {
        public Input Input { get; set; }

        public Player(List<Entity> entities) : base(entities)
        {
            Input = new Input()
            {
                Up = Keys.W,
                Down = Keys.S,
                Left = Keys.A,
                Right = Keys.D
            };
        }

        public override void Update(GameTime gameTime)
        {
            Rotate();
            Shoot(gameTime);
            Move();
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

        private void Rotate()
        {
            foreach (Entity e in entities)
                e.RotateTo(Mouse.GetState().Position.ToVector2());
        }

        private void Move() //TODO(lowprio): remove vector 2 instanciation from angle calculation (inefficient, high computational req)
        {
            if (Input == null)
                return;
            
            if ((Keyboard.GetState().IsKeyDown(Input.Up) ^ Keyboard.GetState().IsKeyDown(Input.Down)) || (Keyboard.GetState().IsKeyDown(Input.Left) ^ Keyboard.GetState().IsKeyDown(Input.Right)))
            {
                Vector2 accelerationVector = Vector2.Zero;
                if (Keyboard.GetState().IsKeyDown(Input.Up))
                {
                    accelerationVector.X += (float)Math.Cos((double)MathHelper.ToRadians(90));
                    accelerationVector.Y += (float)Math.Sin((double)MathHelper.ToRadians(-90));
                }
                else if (Keyboard.GetState().IsKeyDown(Input.Down))
                {
                    accelerationVector.X += (float)Math.Cos((double)MathHelper.ToRadians(270));
                    accelerationVector.Y += (float)Math.Sin((double)MathHelper.ToRadians(-270));
                }
                if (Keyboard.GetState().IsKeyDown(Input.Left))
                {
                    accelerationVector.X += (float)Math.Cos((double)MathHelper.ToRadians(180));
                    accelerationVector.Y += (float)Math.Sin((double)MathHelper.ToRadians(-180));
                }
                else if (Keyboard.GetState().IsKeyDown(Input.Right))
                {
                    accelerationVector.X += (float)Math.Cos(0);
                    accelerationVector.Y += (float)Math.Sin(0);
                }
                accelerationVector.Normalize();
                foreach (Entity e in entities)
                    e.Accelerate(accelerationVector, e.Thrust);
            }
        }
    }
}