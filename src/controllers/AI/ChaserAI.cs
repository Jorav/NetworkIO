using Microsoft.Xna.Framework;
using NetworkIO.src.utility;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class ChaserAI : CohesiveController
    {
        private Controller enemy;
        /*public ChaserAI(List<IControllable> collidables, Controller enemy) : base(collidables) //TODO: Change enemy targeting to something smarter
        {
            this.enemy = enemy;
        }*/
        public ChaserAI([OptionalAttribute] Vector2 position, [OptionalAttribute] Controller enemy) : base(position) //TODO: Change enemy targeting to something smarter
        {
            this.enemy = enemy;
            integrateSeperatedEntities = true;
        }
        public override void Update(GameTime gameTime)
        {
            Rotate();
            Accelerate();
            Shoot(gameTime);
            base.Update(gameTime);
        }
        protected void Rotate()
        {
            if(enemy != null)
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

        public override void InteractWith(List<IControllable> controllers)
        {
            base.InteractWith(controllers);
            Controller closest = null;
            foreach (IControllable controllable in controllers)
                if (controllable is Controller c)
                    if (c != this && (c.Team == IDs.TEAM_PLAYER || c.Team == IDs.TEAM_NEUTRAL_HOSTILE))
                        if (closest == null)
                            closest = c;
                        else if ((Position - closest.Position).Length()>(Position-c.Position).Length())
                            closest = c;
            //TODO: Make sure this chases actual entities
            enemy = closest;
        }
        public new static String GetName()
        {
            return "Chasing AI";
        }
    }
}
