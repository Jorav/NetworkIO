using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.collidables;
using NetworkIO.src.entities;
using NetworkIO.src.entities.hulls;
using NetworkIO.src.factories;
using NetworkIO.src.movable;
using NetworkIO.src.utility;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static NetworkIO.src.WorldEntity;

namespace NetworkIO.src.controllers
{
    public class EntityController : Entity, IControllable
    {
        public List<WorldEntity> entities { get; protected set; }
        public CollidableCircle collisionDetector;
        protected float collissionOffset = 100; //TODO make this depend on velocity + other things?
        public new float Radius { get { return radius; } protected set { radius = value; collisionDetector.Radius = value; } }
        protected float radius;
        public override Vector2 Position { get { return position; } set { position = value; collisionDetector.Position = value; } }
        public override float Rotation
        {
            get {
                return rotation;
            }
            set
            {
                float dRotation = Rotation - value;
                foreach(WorldEntity e in entities)
                {
                    Vector2 relativePosition = e.Position - Position;
                    Vector2 newRelativePosition = Vector2.Transform(relativePosition, Matrix.CreateRotationZ(-dRotation));
                    e.Position = newRelativePosition + Position;
                    e.Rotation = value;
                }
                rotation = value;
            }
        }
        public override float Mass { get { float sum = 0; foreach (WorldEntity e in entities) if(!e.IsFiller)sum += e.Thrust; return sum; } set => base.Thrust = value; }
        public override float Thrust { get { float sum = 0; foreach (WorldEntity e in entities) if(!e.IsFiller) sum += e.Thrust; return sum; } set => base.Thrust = value; }

        private List<Queue<Projectile>> projectiles = new List<Queue<Projectile>>();
        //public EntityController(List<IController> collidables) : base(collidables)
        //{}
        public EntityController([OptionalAttribute] Vector2 position, [OptionalAttribute] WorldEntity e) : base(position)
        {//only allow composite for now
            if (position == null)
                position = Vector2.Zero;
            entities = new List<WorldEntity>();
            this.collisionDetector = new CollidableCircle(Position, Radius);
            if(e != null)
                AddEntity(e);
            else
                AddEntity(EntityFactory.Create(position, IDs.COMPOSITE));
            
        }

        public void AddEntity(WorldEntity e)
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
        protected void UpdateRadius() //TODO: Update this to make it more efficient, e.g. by having sorted list
        {
            if (entities.Count == 1)
            {
                if (entities[0] != null)
                    Radius = entities[0].Radius;
            }
            else if (entities.Count > 1)
            {
                float largestDistance = 0;
                foreach (WorldEntity e in entities)
                {
                    if (e.IsCollidable) { 
                        float distance = Vector2.Distance(e.Position, Position) + e.Radius;
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
            foreach (WorldEntity e in entities)
            {
                if (e.IsVisible && !e.IsFiller)
                {
                    sum += e.Position;
                    nrOfLiving++;
                }
            }
            if (nrOfLiving > 0)
                sum /= nrOfLiving;
            Position = sum;
        }
        public override void Shoot(GameTime gameTime)
        {
            foreach (WorldEntity e in entities)
                if (e is Shooter gun)
                    gun.Shoot(gameTime);
        }
        public override void Collide(IControllable controllable)
        {
            if (CollidesWith(controllable)) {//TODO(lowprio): Add predicitive collision e.g. by calculating many steps (make extended collisionobject starting from before calculation and ending where it ended)
                if (controllable is Controller c)
                    foreach (IControllable iC in c.controllables)
                        Collide(iC);
                else if (controllable is EntityController eC)
                {
                    bool collides = false;
                    foreach (WorldEntity e in entities)
                        foreach (WorldEntity eCE in eC.entities)
                            if (e.CollidesWith(eCE))
                            {
                                collides = true;
                                TotalExteriorForce += 0.4f * Physics.CalculateRepulsionForce(e.Position, eCE.Position, e.Elasticity, eCE.Elasticity, Vector2.Distance(e.Position, eCE.Position));
                                TotalExteriorForce += 0.4f * Physics.CalculateRepulsionForce(Position, eC.Position, e.Elasticity, eCE.Elasticity, Vector2.Distance(e.Position, eCE.Position)); //, Vector2.Distance(e.Position, eCE.Position)
                            }
                                
                    if (collides)
                    {
                        TotalExteriorForce += Physics.CalculateBounceForce(Position, Velocity, Mass, eC.Position, eC.Velocity, eC.Mass)*eC.Elasticity;
                    }

                    /*
                    foreach (WorldEntity e in entities)
                        foreach (WorldEntity eE in eC.entities)
                            e.Collide(eE);
                    foreach (Queue<Projectile> pList in projectiles)
                        foreach (Projectile p in pList)
                            foreach (WorldEntity eE in eC.entities) //OBS need adaption for new structure
                                p.Collide(eE);*/
                }
                else if (controllable is WorldEntity wE)
                {
                    bool collides = false;
                    foreach (WorldEntity e in entities)
                        if (e.CollidesWith(wE))
                        {
                            collides = true;
                            TotalExteriorForce += 0.3f * Physics.CalculateRepulsionForce(e.Position, wE.Position, e.Elasticity, wE.Elasticity, Vector2.Distance(e.Position, wE.Position));
                        }
                    if (collides)
                    {
                        TotalExteriorForce += Physics.CalculateBounceForce(Position, Velocity, Mass, wE.Position, wE.Velocity, wE.Mass) * wE.Elasticity;
                    }

                }
                    
            }
        }

        public override void ApplyRepulsion(Entity otherEntity)
        {
            if (Radius + otherEntity.Radius + REPULSIONDISTANCE > Vector2.Distance(Position, otherEntity.Position))
            {
                if (otherEntity is EntityController otherEC)
                {
                    foreach (Entity e1 in entities)
                        foreach (Entity e2 in otherEC.entities)
                        {
                            TotalExteriorForce += CalculateGravitationalRepulsion(e1, e2);
                        }
                }
                else if (otherEntity is WorldEntity otherWE)
                {
                    foreach (Entity e in entities)
                    {
                        TotalExteriorForce += CalculateGravitationalRepulsion(e, otherWE);
                    }
                }
            }
        }

        public override object Clone()
        {
            EntityController cNew = (EntityController)this.MemberwiseClone();
            cNew.projectiles = new List<Queue<Projectile>>();
            cNew.entities = new List<WorldEntity>();
            foreach (WorldEntity e in entities)
                cNew.AddEntity((WorldEntity)e.Clone());
            cNew.collisionDetector = new CollidableCircle(Position, radius);
            return cNew;
        }

        public override void Update(GameTime gameTime)
        {
            //foreach (WorldEntity e in entities)
                //TotalExteriorForce += e.TotalExteriorForce;
            base.Update(gameTime);
            foreach (WorldEntity e in entities)
                e.Position += Velocity;
        }
        public void MoveTo(Vector2 newPosition)
        {
            UpdatePosition();
            Vector2 posChange = newPosition - Position;
            foreach (WorldEntity e in entities)
                e.Position += posChange;
            Position = position + posChange;
        }
        public override bool CollidesWith(IIntersectable c) //NO CHECK DONE, JUST COPY PASTE
        {
            if (c is Controller)
                return collisionDetector.CollidesWith(((Controller)c).collisionDetector);
            else if (c is EntityController)
                return collisionDetector.CollidesWith(((EntityController)c).collisionDetector);
            if (c is WorldEntity && collisionDetector.CollidesWith(((WorldEntity)c).collisionDetector))
                foreach (WorldEntity e in entities)
                    if (e.CollidesWith((WorldEntity)c))
                        return true;
            return false;
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            foreach (WorldEntity e in entities)
                e.Draw(spritebatch);
        }

        public override bool Contains(Vector2 point)
        {
            foreach (WorldEntity e in entities)
                if (e.Contains(point))
                    return true;
            return false;
        }

        /*public override bool EntityContainingInSpace(Vector2 position, Matrix transform)
        {
            foreach (WorldEntity e in entities)
                if (e.EntityContainingInSpace(position, transform))
                    return true;
            return false;
        }*/
        public void AddAvailableLinkDisplays()
        {
            List<WorldEntity> tempEntities = new List<WorldEntity>();
            foreach (WorldEntity e in entities)
            {
                if (!e.IsFiller) { 
                    e.FillEmptyLinks();
                    foreach (WorldEntity ee in e.FillerEntities)
                        if(!entities.Contains(ee))
                            tempEntities.Add(ee);
                }
            }
            foreach (WorldEntity eT in tempEntities)
            {
                bool overlaps = false;
                foreach (WorldEntity eE in entities)
                    if (eT.Contains(eE.Position))
                        overlaps = true;
                if (!overlaps)
                    AddEntity(eT);
                else
                    eT.Links[0].connection.SeverConnection();
             }
        }
        public void ClearAvailableLinks()
        {
            List<WorldEntity> tempEntities = new List<WorldEntity>();
            foreach (WorldEntity e in entities)
            {
                foreach (WorldEntity ee in e.FillerEntities)
                    tempEntities.Add(ee);
            }
            foreach (WorldEntity e in tempEntities)
                entities.Remove(e);
            foreach(WorldEntity e in entities)
                e.ClearEmptyLinks();
        }
        public void ReplaceEntity(WorldEntity eOld, WorldEntity eNew)
        {
            if (entities.Contains(eOld))
            {
                eNew.ConnectTo(eOld.Links[0].connection.Entity, eOld.Links[0].connection);
                entities.Remove(eOld);
                AddEntity(eNew);
            }

        }
        public override IControllable ControllableContainingInSpace(Vector2 position, Matrix transform)
        {
            foreach (WorldEntity e in entities)
                if (e.ControllableContainingInSpace(position, transform) != null)
                    return e;
            return null;
        }
    }
}
