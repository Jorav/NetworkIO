using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.collidables;
using NetworkIO.src.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    class Controller
    {
        public List<Entity> entities;
        protected CollidableCircle collisionDetector;
        protected float collissionOffset = 100; //TODO make this depend on velocity + other things?
        public float Radius { get { return radius; } set { radius = value; collisionDetector.Radius = value; } }
        protected float radius;
        public Vector2 Position { get { return position; } set { position = value; collisionDetector.Position = value; } }
        protected Vector2 position;
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
        public virtual void Update(GameTime gameTime)
        {
            foreach (Entity e in entities)
                if (e.IsVisible)
                    e.Move(gameTime);
            ApplyInternalGravity();
            UpdatePosition();
            UpdateRadius();
        }

        private void ApplyInternalGravity()
        {
            foreach (Entity e1 in entities)
            {
                foreach (Entity e2 in entities)
                {
                    if (e1.IsVisible && e2.IsVisible)
                    {
                        double r = Vector2.Distance(e1.Position, e2.Position);
                        if (e1 != e2)
                        {
                            if (r < 10)
                                r = 10;
                            float res = (float)(e1.AttractionForce * e2.AttractionForce * Math.Pow(r, 1) - e1.RepulsionForce * e2.RepulsionForce / Math.Pow(r, 2));
                            e1.Accelerate(Vector2.Normalize(e2.Position - e1.Position), res);
                        }
                    }
                }
            }
        }

        private void UpdateRadius() //TODO: Update this to make it more efficient, e.g. by having sorted list
        {
            float largestDistance = 100;
            foreach (Entity e in entities)
            {
                if (e.IsVisible)
                {
                    float distance = Vector2.Distance(e.Position, Position);
                    if (distance > largestDistance)
                        largestDistance = distance;
                }
            }
            Radius = largestDistance;
        }

        private void UpdatePosition()
        {
            Vector2 sum = Vector2.Zero;
            int nrOfLiving = 0;
            foreach (Entity e in entities)
                if (e.IsVisible)
                {
                    sum += e.Position;
                    nrOfLiving++;
                }
                    
            sum = sum / nrOfLiving;
            Position = sum;
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Entity e in entities)
                e.Draw(sb);
        }

        public void Collide(Controller c) //Todo: handle subentities collision in e.g. shooter (+projectile)
        {
            if(collisionDetector.collidesWith(c.collisionDetector))//TODO(lowprio): Add predicitive collision e.g. by calculating many steps (make extended collisionobject starting from before calculation and ending where it ended)
                foreach (Entity e in entities)
                    foreach (Entity eC in c.entities)
                        if (e.collidesWith(eC))
                            e.Collide(eC);
            foreach(Queue<Projectile> pList in projectiles)
                foreach(Projectile p in pList)
                    foreach (Entity eC in c.entities)
                        if (p.collidesWith(eC))
                            p.Collide(eC);
        }
    }
}
     