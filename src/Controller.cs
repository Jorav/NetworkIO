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
    public class Controller : Component
    {
        public List<Entity> entities { get; private set; }
        protected CollidableCircle collisionDetector;
        protected float collissionOffset = 100; //TODO make this depend on velocity + other things?
        public float Radius { get { return radius; } protected set { radius = value; collisionDetector.Radius = value; } }
        private float radius;
        public Vector2 Position { get { return position; } protected set { position = value; collisionDetector.Position = value; } }
        private Vector2 position;
        private List<Queue<Projectile>> projectiles;

        public Controller(List<Entity> entities)
        {
            this.entities = entities;
            this.collisionDetector = new CollidableCircle(Position, Radius);
            projectiles = new List<Queue<Projectile>>();
            foreach (Entity e in entities)
                if(e is Shooter s)
                    projectiles.Add(s.Projectiles);
        }

        public void SetEntities(List<Entity> newEntities)
        {
            if(newEntities!=null)
            {
                List<Entity> oldEntities = entities;
                entities = new List<Entity>();
                foreach (Entity e in newEntities)
                    AddEntity(e);
                if (entities.Count == 0)
                    entities = oldEntities;
            }
            
        }

        public void AddEntity(Entity e)
        {
            if (e != null) {
                entities.Add(e);
                if (e is Shooter s)
                    projectiles.Add(s.Projectiles);
            }
        }

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
            ApplyInternalGravity();
        }

        protected virtual void GiveOrders()
        {

        }

        private void UpdateEntities(GameTime gameTime)
        {
            foreach (Entity e in entities)
                if (e.IsVisible)
                    e.Update(gameTime);
        }

        protected void ApplyInternalGravity()
        {
            Vector2 distanceFromController;
            foreach (Entity e1 in entities)
            {
                foreach (Entity e2 in entities)
                {
                    if (e1.IsVisible && e2.IsVisible)
                    {
                        float r = Vector2.Distance(e1.Position, e2.Position);
                        if (e1 != e2 && r<100)
                        {
                            if (r < 10)
                                r = 10;
                            float res = Physics.CalculateGravity(0.1f, 0.1f, 30f, 30f, r);
                            e1.Accelerate(Vector2.Normalize(e2.Position - e1.Position), res);
                            
                        }
                    }
                }
                distanceFromController = Position - e1.Position;
                if (distanceFromController.Length() != 0)
                    e1.Accelerate(Vector2.Normalize(Position - e1.Position), distanceFromController.Length()/1000);
            }
        }

        //TODO: make this work 
        protected void UpdateRadius() //TODO: Update this to make it more efficient, e.g. by having sorted list
        {
            if (entities.Count == 1)
            {
                if (entities[0] != null)
                    Radius = Math.Max(entities[0].Width / 2,entities[0].Height / 2); //OBS, lite godtycklig
            }
            else if (entities.Count > 1)
            {
                float largestDistance = 0;
                foreach (Entity e in entities)
                {
                    if (e.IsVisible)
                    {
                        float distance = Vector2.Distance(e.Position + new Vector2(e.Width, e.Height), Position);
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

        public void Collide(Controller c) //Todo: handle subentities collision in e.g. shooter (+projectile)
        {
            if(collisionDetector.CollidesWith(c.collisionDetector))//TODO(lowprio): Add predicitive collision e.g. by calculating many steps (make extended collisionobject starting from before calculation and ending where it ended)
                foreach (Entity e in entities)
                    foreach (Entity eC in c.entities)
                        e.Collide(eC);
            foreach(Queue<Projectile> pList in projectiles)
                foreach(Projectile p in pList)
                    foreach (Entity eC in c.entities)
                        p.Collide(eC);
        }

        public virtual object Clone()
        {
            Controller cNew = (Controller)this.MemberwiseClone();
            cNew.entities = new List<Entity>();
            foreach (Entity e in entities)
                cNew.entities.Add((Entity)e.Clone());
            cNew.collisionDetector = new CollidableCircle(Position, radius);
            cNew.projectiles = new List<Queue<Projectile>>();
            foreach (Entity e in entities)
                if (e is Shooter s)
                    projectiles.Add(s.Projectiles);
            return cNew;
        }
    }
}
     