using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.collidables;
using NetworkIO.src.entities;
using NetworkIO.src.entities.hulls;
using NetworkIO.src.factories;
using NetworkIO.src.movable;
using NetworkIO.src.movable.entities;
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

        public override Vector2 Velocity { get { return velocity; } set { if (value.Length() > 100) value = value / value.Length() * 100; velocity = value; } }
        public override Vector2 Position { get { return position; } set {
                Vector2 posChange = value - Position;
                foreach (WorldEntity e in entities)
                    e.Velocity += posChange;
                position = value; collisionDetector.Position = value; } }
        private float oldRotation;
        public override float Rotation
        {
            get {
                return rotation;
            }
            set
            {
                oldRotation = rotation;
                rotation = value;
            }
        }
        public override float Mass { get { float sum = 0; foreach (WorldEntity e in entities) if(!e.IsFiller)sum += e.Thrust; return sum; } set => base.Thrust = value; }
        public override float Thrust { get { float sum = 0; foreach (WorldEntity e in entities) if(!e.IsFiller) sum += e.Thrust; return sum; } set => base.Thrust = value; }

        private List<Queue<Projectile>> projectiles = new List<Queue<Projectile>>();
        public EntityController([OptionalAttribute] Vector2 position, [OptionalAttribute] WorldEntity e) : base(position)
        {//only allow composite for now
            if (position == null)
                position = Vector2.Zero;
            entities = new List<WorldEntity>();
            this.collisionDetector = new CollidableCircle(Position, Radius);
            if(e != null)
            {
                AddEntity(e);
                e.Friction = 0;
            }

            else
            {
                e = EntityFactory.Create(position, IDs.COMPOSITE);
                AddEntity(e);
                e.Friction = 0;
            }
                
            
        }

        /*
         * returns whether an entity was succesfully added
         */
        public bool AddEntity(WorldEntity e)
        {
            if (e != null)
            {
                foreach (WorldEntity ee in entities)
                    if (!ee.IsFiller && ee.CollidesWith(e))
                        return false;
                entities.Add(e);
                if (e is Shooter s)
                    projectiles.Add(s.Projectiles);
                UpdatePosition();
                UpdateRadius();
                e.Friction = 0;
                e.EntityController = this;
                return true;
            }
            return false;
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
            position = sum;
            collisionDetector.Position = position;
        }
        public override void Shoot(GameTime gameTime)
        {
            foreach (WorldEntity e in entities)
                if (e is Shooter gun)
                    gun.Shoot(gameTime);
        }
        public override void Collide(IControllable controllable) //OBS: ADD COLLISSION HADNLING SUPPORT FOR WORLDENTITIES NOT BELONGING TO ENTITY CONTROLLER
        {
            if (controllable is Controller c)
                foreach (IControllable iC in c.controllables)
                        Collide(iC);
            else if (controllable is EntityController eC)
            {
                if (CollidesWith(controllable))
                {
                    foreach (WorldEntity e in entities)
                        foreach (WorldEntity eCE in eC.entities)
                            Collide(e, eCE);
                }/*
                foreach (Queue<Projectile> pList in projectiles)
                    foreach (Projectile p in pList)
                        foreach (WorldEntity eE in eC.entities) //OBS need adaption for new structure
                            Collide(p, eE);*/
            }
            else if (controllable is WorldEntity wE)
            {
                    foreach (WorldEntity e in entities)
                        Collide(e, wE);
                    /*
                foreach (Queue<Projectile> pList in projectiles)
                    foreach (Projectile p in pList)
                        Collide(p, wE);*/
            }
            
            
        }
        private void Collide(WorldEntity e, WorldEntity eCE)
        {
            bool collides = false;
            if (e.CollidesWith(eCE))
            {
                collides = true;
                TotalExteriorForce += Physics.CalculateCollissionRepulsion(e.Position-e.Velocity, eCE.Position-eCE.Velocity, e.Velocity*e.Mass, eCE.Velocity*e.Mass);
                TotalExteriorForce += Physics.CalculateOverlapRepulsion(e.Position - e.Velocity, eCE.Position - eCE.Velocity, e.Radius);
                TotalExteriorForce += 0.7f * Physics.CalculateOverlapRepulsion(Position - e.Velocity, eCE.Position - eCE.Velocity, e.Radius);
            }
            else
            {
                Vector2 distanceBeforeMoving = e.Position - e.Velocity - (eCE.Position - eCE.Velocity);
                Vector2 distance = e.Position - eCE.Position;
                if (Vector2.Dot(eCE.Velocity, distanceBeforeMoving) > Vector2.Dot(e.Velocity, distanceBeforeMoving) + distanceBeforeMoving.Length() && eCE.CollidesWithDuringMove(e))//if they move
                {
                    collides = true;
                    TotalExteriorForce += Physics.CalculateCollissionRepulsion(e.Position - e.Velocity, eCE.Position, e.Velocity * e.Mass, eCE.Velocity * e.Mass);
                    TotalExteriorForce += Physics.CalculateOverlapRepulsion(e.Position - e.Velocity, eCE.Position, e.Radius);
                    TotalExteriorForce += 0.7f * Physics.CalculateOverlapRepulsion(Position, eCE.Position, e.Radius);
                }
                else if (Vector2.Dot(e.Velocity, -distanceBeforeMoving) > Vector2.Dot(eCE.Velocity, -distanceBeforeMoving) + distanceBeforeMoving.Length() && eCE.CollidesWithDuringMove(e))
                {
                    collides = true;
                    TotalExteriorForce += Physics.CalculateCollissionRepulsion(e.Position, eCE.Position - eCE.Velocity, e.Velocity * e.Mass, eCE.Velocity * e.Mass);
                    TotalExteriorForce += Physics.CalculateOverlapRepulsion(e.Position, eCE.Position - eCE.Velocity, e.Radius);
                    TotalExteriorForce += 0.7f * Physics.CalculateOverlapRepulsion(Position, eCE.Position, e.Radius);
                }
            }
            if (collides && e is Spike s)
            {
                s.Collide(eCE);
            }
        }

        public void CollideProjectiles(IControllable collidable)
        {
            if (collidable is Controller c)
                foreach (IControllable cc in c.controllables)
                    CollideProjectiles(cc);
            else if(collidable is EntityController ec)
                foreach (Queue<Projectile> pList in projectiles)
                    foreach (Projectile p in pList)
                        foreach(WorldEntity e in ec.entities)
                            p.Collide(e);
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
            MoveAndRotateEntities();
            base.Update(gameTime);
            foreach (WorldEntity e in entities)
                e.Update(gameTime);
        }

        private void MoveAndRotateEntities()
        {
            float dRotation = oldRotation - Rotation;
            foreach (WorldEntity e in entities)
            {
                Vector2 relativePosition = e.Position - Position;
                Vector2 newRelativePosition = Vector2.Transform(relativePosition, Matrix.CreateRotationZ(-dRotation));
                e.Velocity = newRelativePosition - relativePosition;
                e.Rotation = Rotation;

            }
            oldRotation = rotation;
        }

        public void MoveTo(Vector2 newPosition)
        {
            //UpdatePosition();
            Vector2 posChange = newPosition - Position;
            foreach (WorldEntity e in entities)
                e.Position += posChange;
            position = position + posChange;
            collisionDetector.Position = position;
        }
        public override bool CollidesWith(IIntersectable c) //NO CHECK DONE, JUST COPY PASTE
        {
            if (c is Controller)
                return collisionDetector.CollidesWith(((Controller)c).collisionDetector);
            else if (c is EntityController)
                return collisionDetector.CollidesWith(((EntityController)c).collisionDetector);
            if (c is WorldEntity && Vector2.Distance(c.Position, Position) < Radius/*collisionDetector.CollidesWith(((WorldEntity)c).collisionDetector*/)
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
                    if (eT.CollidesWith(eE))//eT.Contains(eE.Position) || eE.Contains(eT.Position))
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
        public bool ReplaceEntity(WorldEntity eOld, WorldEntity eNew)
        {
            if (entities.Contains(eOld))
            {
                eNew.ConnectTo(eOld.Links[0].connection.Entity, eOld.Links[0].connection);
                entities.Remove(eOld);
                if (!AddEntity(eNew))
                {
                    eOld.ConnectTo(eNew.Links[0].connection.Entity, eNew.Links[0].connection);
                    AddEntity(eOld);
                    return false;
                }
                return true;
            }
            return false;
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
