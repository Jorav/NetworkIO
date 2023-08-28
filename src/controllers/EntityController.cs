using NetworkIO.src.entities;
using NetworkIO.src.entities.hulls;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class EntityController : Controller
    {
        private List<Queue<Projectile>> projectiles = new List<Queue<Projectile>>();
        public EntityController(List<ICollidable> collidables) : base(collidables)
        {
        }

        public override void AddCollidable(ICollidable c)
        {
            if (c != null && c is Entity)
            {
                collidables.Add(c);
                if (c is Shooter s)
                    projectiles.Add(s.Projectiles);
                UpdatePosition();
                UpdateRadius();
            }
        }

        public void AddEntity(Entity e)
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
        public override void Collide(ICollidable collidable)
        {
            base.Collide(collidable);
            if (collidable is EntityController cc)
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
    }
}
