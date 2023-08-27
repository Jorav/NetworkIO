using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.collidables;
using NetworkIO.src.entities;
using NetworkIO.src.menu;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    public abstract class Controller : IComponent, ICollidable
    {
        public List<Entity> entities { get; protected set; }
        public CollidableCircle collisionDetector;
        protected float collissionOffset = 100; //TODO make this depend on velocity + other things?
        public float Radius { get { return radius; } protected set { radius = value; collisionDetector.Radius = value; } }
        protected float radius;
        public Vector2 Position { get { return position; } set { position = value; collisionDetector.Position = value; } }
        protected Vector2 position;

        public Controller(List<Entity> entities)
        {
            this.collisionDetector = new CollidableCircle(Position, Radius);
            SetEntities(entities);
        }

        public abstract void SetEntities(List<Entity> newEntities);

        public void MoveTo(Vector2 newPosition)
        {
            UpdatePosition();
            Vector2 posChange = newPosition - Position;
            foreach (Entity e in entities)
                e.Position += posChange;
            Position = position + posChange;
        }

        public virtual void Update(GameTime gameTime)
        {
            UpdateEntities(gameTime);
            UpdatePosition();
            UpdateRadius();
        }

        private void UpdateEntities(GameTime gameTime)
        {
            foreach (Entity e in entities)
                if (e.IsVisible)
                    e.Update(gameTime);
        }

        //TODO: make this work 
        protected void UpdateRadius() //TODO: Update this to make it more efficient, e.g. by having sorted list
        {
            if (entities.Count == 1)
            {
                if (entities[0] != null)
                    Radius = (float)Math.Sqrt(Math.Pow(entities[0].Width / 2, 2) + Math.Pow(entities[0].Height / 2, 2)); //OBS, lite godtycklig
            }
            else if (entities.Count > 1)
            {
                float largestDistance = 0;
                foreach (Entity e in entities)
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
            foreach (Entity e in entities)
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
            foreach (Entity e in entities)
                e.Draw(sb);
        }

        public virtual void Collide(Controller c)
        {
            if (CollidesWith(c))//TODO(lowprio): Add predicitive collision e.g. by calculating many steps (make extended collisionobject starting from before calculation and ending where it ended)
                foreach (Entity e in entities)
                    foreach (Entity eC in c.entities)
                        e.Collide(eC);
        }

        public virtual object Clone()
        {
            Controller cNew = (Controller)this.MemberwiseClone();
            cNew.entities = new List<Entity>();
            foreach (Entity e in entities)
                cNew.entities.Add((Entity)e.Clone());
            cNew.collisionDetector = new CollidableCircle(Position, radius);
            return cNew;
        }

        public bool CollidesWith(ICollidable c)
        {
            if(c is Controller)
                return collisionDetector.CollidesWith(((Controller)c).collisionDetector);
            if (c is Entity && collisionDetector.CollidesWith(((Entity)c).collisionDetector))
            {
                bool collides = false;
                foreach (Entity e in entities)
                    if (e.CollidesWith((Entity)c))
                        collides = true;
                return collides;
            }
            return false;
        }
    }
}
     