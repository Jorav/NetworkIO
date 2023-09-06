using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NetworkIO.src.controllers;
using NetworkIO.src.entities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NetworkIO.src
{
    public class Player : CollidablesController
    {
        public Input Input { get; set; }
        public Camera Camera { get; private set; }
        public bool actionsLocked;
        
        
        public Player(List<ICollidable> collidables, Input input) : base(collidables)
        {
            this.Input = input;
            Camera = new Camera(this);
            Input.Camera = Camera;
        }
        public Player(Input input, [OptionalAttribute]Vector2 position) : base()
        {
            this.Input = input;
            Camera = new Camera(this);
            Input.Camera = Camera;
        }

        public override void Update(GameTime gameTime)
        {
            if (!actionsLocked)
            {
                RotateTo(Input.MousePositionGameCoords);
                Accelerate();
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    Shoot(gameTime);
            }
            
            Camera.Update();
            base.Update(gameTime);
            /*
             * Rotate, calculate course, check collisions, update course, move, base.update
             */
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
                foreach (EntityController eC in collidables)
                    eC.Accelerate(accelerationVector);
            }
        }

        //***NOT GUARANTEED TO WORK***
        public override object Clone()
        {
            Player pNew = (Player)base.Clone();
            pNew.Camera = new Camera(this);
            return pNew;
        }
    }
}