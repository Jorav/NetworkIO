using Microsoft.Xna.Framework;
using NetworkIO.src.entities.hulls;
using NetworkIO.src.movable;
using NetworkIO.src.utility;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class CohesiveController : Controller
    {
        protected bool integrateSeperatedEntities = false;

        public CohesiveController(List<IControllable> controllables, IDs team = IDs.TEAM_AI) : base(controllables, team) { }
        public CohesiveController([OptionalAttribute] Vector2 position, IDs team = IDs.TEAM_AI) : base(position, team) { }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            ApplyInternalGravity();
            ApplyInternalRepulsion();
        }

        protected void ApplyInternalGravity()
        {
            Vector2 distanceFromController;
            foreach (IControllable c1 in Controllables)
            {
                distanceFromController = Position - c1.Position;
                if (distanceFromController.Length() > c1.Radius)
                    c1.Accelerate(Vector2.Normalize(Position - c1.Position), (float)Math.Pow(((distanceFromController.Length() - c1.Radius) / AverageDistance()) / 2 * c1.Mass, 2));
            }
        }
        public void ApplyInternalRepulsion()
        {
            foreach (IControllable c1 in Controllables)
            {
                foreach (IControllable c2 in Controllables)//TODO: only allow IsCollidable to affect this?
                {
                    if (c1 != c2 && c1 is Entity e1 && c2 is Entity e2)
                        e1.ApplyRepulsion(e2);
                }
            }
        }
        protected override void AddSeperatedEntities()
        {
            if (integrateSeperatedEntities)
            {
                List<EntityController> seperatedEntities = new List<EntityController>();
                foreach (IControllable c in Controllables)
                    if (c is EntityController ec)
                        foreach (EntityController ecSeperated in ec.SeperatedEntities)
                        {
                            if (ecSeperated.Controllables.Count == 1 && !(ecSeperated.Controllables[0] is Composite))
                                ;//((WorldEntity)(ecSeperated.Controllables[0])).Die();
                            else
                                seperatedEntities.Add(ecSeperated);
                        }
                foreach (EntityController ec in seperatedEntities)
                {
                    AddControllable(ec);
                }
                foreach (IControllable c in Controllables)
                    if (c is EntityController ec)
                        ec.SeperatedEntities.Clear();
            }
                
            else
                base.AddSeperatedEntities();
        }
    }
}
