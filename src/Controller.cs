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
        protected void UpdateRadius() //TODO: Update this to make it more efficient, e.g. by having sorted list
        {
            if (collidables.Count == 1)
            {
                if (collidables[0] != null)
                    Radius = collidables[0].Radius;
            }
            else if (collidables.Count > 1)
            {
                float largestDistance = 0;
                foreach (Entity e in collidables)
                {
                    if (e.IsVisible)
                    {
                        float distance = Vector2.Distance(e.Position, Position)+(float)Math.Sqrt(Math.Pow(e.Width/2,2)+ Math.Pow(e.Height/2, 2));
                        if (distance > largestDistance)
                            largestDistance = distance;
                    }
                }
                Radius = largestDistance;
            }
        }
        
        protected void UpdatePosition()
        {
            Vector2 sum = Vector2.Zero;
            int nrOfLiving = 0;
            foreach (Entity e in collidables)
                if (e.IsVisible)
                {
                    sum += e.Position;
                    nrOfLiving++;
                }
            if(nrOfLiving > 0)  
                sum /= nrOfLiving;
            Position = sum;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            foreach (Entity e in collidables)
                e.Draw(sb);
        }

        public virtual void Collide(ICollidable collidable) // OBS - THIS NEEDS TO BE ADAPTED FOR ICOLLIDABLE
        {
            if (CollidesWith(collidable))//TODO(lowprio): Add predicitive collision e.g. by calculating many steps (make extended collisionobject starting from before calculation and ending where it ended)
                foreach (ICollidable c in collidables)
                    c.Collide(collidable);
        }

        public virtual object Clone()
        {
            Controller cNew = (Controller)this.MemberwiseClone();
            cNew.collidables = new List<ICollidable>();
            foreach (Entity e in collidables)
                cNew.collidables.Add((Entity)e.Clone());
            foreach (EntityController ec in collidables)
                cNew.collidables.Add((Entity)ec.Clone());
            foreach (CollidablesController cc in collidables)
                cNew.collidables.Add((Entity)cc.Clone());
            cNew.collisionDetector = new CollidableCircle(Position, radius);
            return cNew;
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
    }
}
     