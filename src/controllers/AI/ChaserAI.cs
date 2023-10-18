using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class ChaserAI : Controller
    {
        private Controller enemy;
        /*public ChaserAI(List<IControllable> collidables, Controller enemy) : base(collidables) //TODO: Change enemy targeting to something smarter
        {
            this.enemy = enemy;
        }*/
        public ChaserAI([OptionalAttribute] Vector2 position, [OptionalAttribute] Controller enemy) : base(position) //TODO: Change enemy targeting to something smarter
        {
            this.enemy = enemy;
        }
        public override void Update(GameTime gameTime)
        {
            Rotate();
            Accelerate();
            base.Update(gameTime);
        }
        protected void Rotate()
        {
            //RotateTo(enemy.controllables[0].Position);
            RotateTo(enemy.Position);
        }
        protected void Accelerate()
        {
            if (enemy != null)
            {
                foreach (IControllable c in Controllables)
                {
                    Vector2 accelerationVector;
                    /*if(enemy.controllables[0] is WorldEntity)
                        accelerationVector = enemy.controllables[0].Position + ((WorldEntity)(enemy.controllables[0])).Velocity - (c.Position/* + c.Velocity);
                    //accelerationVector = enemy.controllables[0].Position + ((WorldEntity)(enemy.controllables[0])).Velocity - (c.Position/* + c.Velocity);
                    else
                        accelerationVector = enemy.controllables[0].Position - (c.Position/* + c.Velocity);*/

                    accelerationVector = enemy.Position - (c.Position/* + c.Velocity*/);


                    accelerationVector.Normalize();
                    c.Accelerate(accelerationVector);
                }
            }
        }
        public new static String GetName()
        {
            return "Chasing AI";
        }
    }
}
