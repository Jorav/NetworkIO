using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NetworkIO.src.controllers
{
    class ChaserAI : EntityController
    {
        private Controller enemy;
        /*public ChaserAI(List<IControllable> collidables, Controller enemy) : base(collidables) //TODO: Change enemy targeting to something smarter
        {
            this.enemy = enemy;
        }*/
        public ChaserAI([OptionalAttribute] Vector2 position, Controller enemy) : base(position) //TODO: Change enemy targeting to something smarter
        {
            this.enemy = enemy;
            entities[0].EntityController = this;
        }
        public override void Update(GameTime gameTime)
        {
            Rotate();
            Accelerate();
            base.Update(gameTime);
        }
        protected void Rotate()
        {
            RotateTo(enemy.controllables[0].Position);
        }
        protected void Accelerate()
        {
            foreach (WorldEntity e in entities)
            {
                Vector2 accelerationVector;
                if(enemy.controllables[0] is WorldEntity)
                    accelerationVector = enemy.controllables[0].Position + ((WorldEntity)(enemy.controllables[0])).Velocity - (e.Position + e.Velocity);
                else
                    accelerationVector = enemy.controllables[0].Position - (e.Position + e.Velocity);

                accelerationVector.Normalize();
                base.Accelerate(accelerationVector);
            }
        }
    }
}
