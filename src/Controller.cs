using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.collidables;
using NetworkIO.src.controllers;
using NetworkIO.src.entities;
using NetworkIO.src.menu;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    public abstract class Controller : IComponent, ICollidable
    {
        public List<ICollidable> collidables { get; protected set; }
        public CollidableCircle collisionDetector;
        protected float collissionOffset = 100; //TODO make this depend on velocity + other things?
        public float Radius { get { return radius; } protected set { radius = value; collisionDetector.Radius = value; } }
        protected float radius;
        public Vector2 Position { get { return position; } set { position = value; collisionDetector.Position = value; } }
        protected Vector2 position;

        public Controller(List<ICollidable> collidables)
        {
            this.collisionDetector = new CollidableCircle(Position, Radius);
            SetCollidables(collidables);
        }

        public Controller()
        {
            this.collisionDetector = new CollidableCircle(Position, Radius);
        }

        public virtual void SetCollidables(List<ICollidable> newCollidables)
        {
            if (newCollidables != null)
            {
                List<ICollidable> oldCollidables = collidables;
                collidables = new List<ICollidable>();
                foreach (ICollidable c in newCollidables)
                    AddCollidable(c);
                if (collidables.Count == 0)
                {
                    collidables = oldCollidables;
                }
            }
        }

        public abstract void AddCollidable(ICollidable c);

        public abstract void Shoot(GameTime gameTime);

        public void RotateTo(Vector2 position)
        {
            foreach (ICollidable c in collidables)
                c.RotateTo(position);
        }

        public void MoveTo(Vector2 newPosition)
        {
            UpdatePosition();
            Vector2 posChange = newPosition - Position;
            foreach (ICollidable c in collidables)
                c.Position += posChange;
            Position = position + posChange;
        }

        public virtual void Update(GameTime gameTime)
        {
            UpdateCollidables(gameTime);
            UpdatePosition();
            UpdateRadius();
        }

        private void UpdateCollidables(GameTime gameTime)
        {
            foreach (ICollidable c in collidables)
                c.Update(gameTime);
        }

        //TODO: make this work 
        protected void UpdateRadius() //TODO: Update this to make it more efficient, e.g. by having sorted list, TODO: only allow IsCollidable to affect this?
        {
            if (collidables.Count == 1)
            {
                if (collidables[0] != null)
                    Radius = collidables[0].Radius;
            }
            else if (collidables.Count > 1)
            {
                float largestDistance = 0;
                foreach (ICollidable c in collidables)
                {
                    float distance = Vector2.Distance(c.Position, Position)+c.Radius;
                    if (distance > largestDistance)
                        largestDistance = distance;
                }
                Radius = largestDistance;
            }
        }
        
        protected void UpdatePosition() //TODO: only allow IsCollidable to affect this?
        {
            Vector2 sum = Vector2.Zero;
            int nrOfLiving = 0;
            foreach (ICollidable c in collidables) 
            { 
                sum += c.Position;
                nrOfLiving++;
            }
            if(nrOfLiving > 0)  
                sum /= nrOfLiving;
            Position = sum;
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (ICollidable c in collidables)
                c.Draw(sb);
        }

        public virtual void Collide(ICollidable collidable) // OBS - THIS NEEDS TO BE ADAPTED FOR ICOLLIDABLE
        {
            if (CollidesWith(collidable))//TODO(lowprio): Add predicitive collision e.g. by calculating many steps (make extended collisionobject starting from before calculation and ending where it ended)
                foreach (ICollidable c in collidables)
                    c.Collide(collidable);
        }

        public bool CollidesWith(IIntersectable c)
        {
            if(c is Controller)
                return collisionDetector.CollidesWith(((Controller)c).collisionDetector);
            if (c is Entity && collisionDetector.CollidesWith(((Entity)c).collisionDetector))
                foreach (Entity e in collidables)
                    if (e.CollidesWith((Entity)c))
                        return true;
            return false;
        }

        public void Accelerate(Vector2 directionalVector, float thrust)
        {
            foreach (ICollidable c in collidables)
                c.Accelerate(directionalVector, thrust);
        }

        public virtual object Clone()
        {
            Controller cNew = (Controller)this.MemberwiseClone();
            cNew.collidables = new List<ICollidable>();
            foreach (ICollidable c in collidables)
                cNew.AddCollidable((ICollidable)c.Clone());
            cNew.collisionDetector = new CollidableCircle(Position, radius);
            return cNew;
        }

    }
}
     