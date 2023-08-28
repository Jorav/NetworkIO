using Microsoft.Xna.Framework;
using NetworkIO.src.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class CollidablesController : Controller
    {
        private List<Queue<Projectile>> projectiles = new List<Queue<Projectile>>();
        public CollidablesController(List<ICollidable> collidables) : base(collidables)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            ApplyInternalGravity();
        }

        protected void AddEntity(Entity e)
        {
            if (e != null)
            {
                collidables.Add(e);
                if (e is Shooter s)
                    projectiles.Add(s.Projectiles);
                UpdatePosition();
                UpdateRadius();
            }

        }

        public override void AddCollidable(ICollidable c)
        {
            if (c != null)
            {
                if (c is Entity e)
                    AddEntity(e);
                else
                {
                    collidables.Add(c);
                    UpdatePosition();
                    UpdateRadius();
                }
            }

        }

        public override void SetCollidables(List<ICollidable> newCollidables)
        {
            if (newCollidables != null) { 
            List<ICollidable> oldCollidables = collidables;
            List<Queue<Projectile>> oldProjectiles = projectiles;
            projectiles = new List<Queue<Projectile>>();
            base.SetCollidables(newCollidables);
            if (collidables != null && collidables.Count == 0)
                    projectiles = oldProjectiles;
            }
        }
    /*
        public void SetEntities(List<Entity> newEntities)
        {
            if (newEntities != null)
            {
                List<Queue<Projectile>> oldProjectiles = projectiles;
                projectiles = new List<Queue<Projectile>>();
                List<ICollidable> oldEntities = collidables;
                collidables = new List<ICollidable>();
                foreach (Entity e in newEntities)
                    AddEntity(e);
                if (collidables.Count == 0) { 
                    collidables = oldEntities;
                    projectiles = oldProjectiles;
                }
            }

        }
    */

        protected void ApplyInternalGravity()
        {
            Vector2 distanceFromController;
            foreach (Entity e1 in collidables)
            {
                foreach (Entity e2 in collidables)
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

        public override void Collide(ICollidable collidable)
        {
            base.Collide(collidable);
            if(collidable is CollidablesController cc)
            foreach (Queue<Projectile> pList in projectiles)
                foreach (Projectile p in pList)
                    foreach (Entity eC in cc.collidables) //OBS need adaption for new structure
                        p.Collide(eC);
        }
        public void Collide(Controller c) //Todo: handle subentities collision in e.g. shooter (+projectile)
        {
            if (CollidesWith(c))//TODO(lowprio): Add predicitive collision e.g. by calculating many steps (make extended collisionobject starting from before calculation and ending where it ended)
                foreach (Entity e in collidables)
                    foreach (Entity eC in c.collidables)
                        e.Collide(eC);
            foreach (Queue<Projectile> pList in projectiles)
                foreach (Projectile p in pList)
                    foreach (Entity eC in c.collidables)
                        p.Collide(eC);
        }

        protected virtual void GiveOrders()
        {

        }

        public override object Clone()
        {
            CollidablesController cNew = (CollidablesController)this.MemberwiseClone();
            cNew.projectiles = new List<Queue<Projectile>>();
            foreach (Entity e in collidables)
                if (e is Shooter s)
                    cNew.projectiles.Add(s.Projectiles);
            return cNew;
        }
    }
}
