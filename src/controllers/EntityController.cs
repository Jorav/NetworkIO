using Microsoft.Xna.Framework;
using NetworkIO.src.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class EntityController : Controller
    {
        private List<Queue<Projectile>> projectiles;
        public EntityController(List<Entity> entities) : base(entities)
        {
        }

        public override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);
            ApplyInternalGravity();
        }

        public void AddEntity(Entity e)
        {
            if (e != null)
            {
                entities.Add(e);
                if (e is Shooter s)
                    projectiles.Add(s.Projectiles);
                UpdatePosition();
                UpdateRadius();
            }

        }

        public override void SetEntities(List<Entity> newEntities)
        {
            if (newEntities != null)
            {
                List<Queue<Projectile>> oldProjectiles = projectiles;
                projectiles = new List<Queue<Projectile>>();
                List<Entity> oldEntities = entities;
                entities = new List<Entity>();
                foreach (Entity e in newEntities)
                    AddEntity(e);
                if (entities.Count == 0) { 
                    entities = oldEntities;
                    projectiles = oldProjectiles;
                }
            }

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
                        if (e1 != e2 && r < 100 && r != 0)
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
                    e1.Accelerate(Vector2.Normalize(Position - e1.Position), distanceFromController.Length() / 1000);
            }
        }

        public override void Collide(Controller c) //Todo: handle subentities collision in e.g. shooter (+projectile)
        {
            base.Collide(c);
            foreach (Queue<Projectile> pList in projectiles)
                foreach (Projectile p in pList)
                    foreach (Entity eC in c.entities)
                        p.Collide(eC);
        }

        protected virtual void GiveOrders()
        {

        }

        public override object Clone()
        {
            EntityController cNew = (EntityController)this.MemberwiseClone();
            cNew.projectiles = new List<Queue<Projectile>>();
            foreach (Entity e in entities)
                if (e is Shooter s)
                    cNew.projectiles.Add(s.Projectiles);
            return cNew;
        }
    }
}
