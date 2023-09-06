using Microsoft.Xna.Framework;
using NetworkIO.src.collidables;
using NetworkIO.src.entities;
using NetworkIO.src.entities.hulls;
using NetworkIO.src.factories;
using NetworkIO.src.utility;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class CollidablesController : Controller
    {
        private List<Queue<Projectile>> projectiles = new List<Queue<Projectile>>();
        public CollidablesController(List<ICollidable> collidables) : base(collidables)
        {
        }

        public CollidablesController([OptionalAttribute]Vector2 position) : base(null)
        {
            if (position == null)
                position = Vector2.Zero;
            SetCollidables(new List<ICollidable>() { new EntityController(position) });
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

        public override void Shoot(GameTime gameTime)
        {
            foreach (Controller c in collidables)
                c.Shoot(gameTime);
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
            foreach (ICollidable c1 in collidables)
            {
                foreach (ICollidable c2 in collidables)//TODO: only allow IsCollidable to affect this?
                {
                    float r = Vector2.Distance(c1.Position, c2.Position);
                    if (c1 != c2 && r < 100 && r != 0)
                    {
                        if (r < 10)
                            r = 10;
                        float res = Physics.CalculateGravity(0.1f, 0.1f, 30f, 30f, r);
                        c1.Accelerate(Vector2.Normalize(c2.Position - c1.Position), res);
                    }
                }
                distanceFromController = Position - c1.Position;
                if (distanceFromController.Length() != 0)
                    c1.Accelerate(Vector2.Normalize(Position - c1.Position), distanceFromController.Length() / 1000);
            }
        }

        public override void Collide(ICollidable collidable)
        {
            base.Collide(collidable);
            if(collidable is CollidablesController cc)
            foreach (Queue<Projectile> pList in projectiles)
                foreach (Projectile p in pList)
                    foreach (ICollidable c in cc.collidables) //OBS need adaption for new structure
                        p.Collide(c);
        }

        protected virtual void GiveOrders(){}

        public override object Clone()
        {
            CollidablesController cNew = (CollidablesController)this.MemberwiseClone();
            cNew.projectiles = new List<Queue<Projectile>>();
            cNew.collidables = new List<ICollidable>();
            foreach (ICollidable c in collidables)
                cNew.AddCollidable((ICollidable)c.Clone());
            cNew.collisionDetector = new CollidableCircle(Position, radius);
            return cNew;
        }

        /*public void Collide(Controller c) //Todo: handle subentities collision in e.g. shooter (+projectile)
        {
            if (CollidesWith(c))//TODO(lowprio): Add predicitive collision e.g. by calculating many steps (make extended collisionobject starting from before calculation and ending where it ended)
                foreach (Entity e in collidables)
                    foreach (Entity eC in c.collidables)
                        e.Collide(eC);
            foreach (Queue<Projectile> pList in projectiles)
                foreach (Projectile p in pList)
                    foreach (Entity eC in c.collidables)
                        p.Collide(eC);
        }*/
    }
}
