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
        #region Properties
        public List<WorldEntity> Entities { get; protected set; }
        public List<EntityController> SeperatedEntities { get; set; }
        public CollidableCircle collisionDetector;
        protected float collissionOffset = 100; //TODO make this depend on velocity + other things?
        public new float Radius { get { return radius; } protected set { radius = value; collisionDetector.Radius = value; } }
        protected float radius;

        public override Vector2 Velocity { get { return velocity; } set { if (value.Length() > 100) value = value / value.Length() * 100; velocity = value; } }
        public override Vector2 Position { get { return position; } set {
                Vector2 posChange = value - Position;
                foreach (WorldEntity e in Entities)
                    e.Position += posChange;
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
        public override float Mass { get { float sum = 0; foreach (WorldEntity e in Entities) if (!e.IsFiller && e.IsAlive) sum += e.Mass; return sum; } }
        public override float Thrust { get { float sum = 0; foreach (WorldEntity e in Entities) if (!e.IsFiller && e.IsAlive) sum += e.Thrust; return sum; } }

        private List<Queue<Projectile>> projectiles = new List<Queue<Projectile>>();
        #endregion

        public EntityController([OptionalAttribute] Vector2 position, [OptionalAttribute] WorldEntity e) : base(position)
        {//only allow composite for now
            SeperatedEntities = new List<EntityController>();
            if (position == null)
                position = Vector2.Zero;
            Entities = new List<WorldEntity>();
            this.collisionDetector = new CollidableCircle(Position, Radius);
            Position = position;
            if (e != null)
            {
                AddEntity(e);
            }

            else
            {
                e = EntityFactory.Create(position, IDs.COMPOSITE);
                e.Position = position;
                AddEntity(e);
            }
        }
        public EntityController(WorldEntity[] entities, float rotation, [OptionalAttribute] Vector2 position) : base(position)
        {//only allow composite for now
            this.Rotation = rotation;
            SeperatedEntities = new List<EntityController>();
            this.Entities = new List<WorldEntity>();
            if (position == null)
                position = Vector2.Zero;
            this.collisionDetector = new CollidableCircle(Position, Radius);
            Position = position;
            if (entities != null && entities.Length > 0)
                foreach (WorldEntity e in entities)
                    AddEntity(e);
            else
            {
                AddEntity(EntityFactory.Create(position, IDs.COMPOSITE));
            }

            MoveAndRotateEntities();
        }
        #region Methods
        /**
         * returns whether an entity was succesfully added
         */
        public bool AddEntity(WorldEntity e)
        {
            if (e != null)
            {
                foreach (WorldEntity ee in Entities)
                    if (!ee.IsFiller && ee.CollidesWith(e))
                        return false;
                Entities.Add(e);
                if (e is Shooter s)
                    projectiles.Add(s.Projectiles);
                e.Friction = 0;
                e.EntityController = this;
                UpdatePosition();
                UpdateRadius();
                e.Rotation = Rotation;
                ConnectToOthers(e);
                return true;
            }
            return false;
        }
        
        /**
         * returns whether an entity was succesfully removed
         */
        public bool RemoveEntity(WorldEntity e)
        {
            if (e != null && Entities.Remove(e))
            {
                foreach (Link l in e.Links)
                    if (!l.ConnectionAvailable && l.connection.Entity.Links.Count == 1)
                        ;// RemoveEntity(l.connection.Entity);
                if (e is Shooter s)
                    projectiles.Remove(s.Projectiles);
                
                foreach (Link l in e.Links) //remove filler links
                    if (!l.ConnectionAvailable)
                    {
                        if(l.connection.Entity.IsFiller)
                            Entities.Remove(l.connection.Entity);
                        l.SeverConnection();
                    }
                List<HashSet<WorldEntity>> connectedEntities = GetSetsOfEntities();
                for (int i = 1; i < connectedEntities.Count; i++)
                {
                    WorldEntity[] tempEntities = new WorldEntity[connectedEntities[i].Count];
                    connectedEntities[i].CopyTo(tempEntities);
                    foreach (WorldEntity eSeperated in tempEntities)
                        Entities.Remove(eSeperated);
                    EntityController ec = new EntityController(tempEntities, Rotation);
                    
                    SeperatedEntities.Add(ec);
                }
                UpdatePosition();
                UpdateRadius();
                return true;
            }
            else
            {
                bool removed = false;
                foreach (EntityController ec in SeperatedEntities)
                    if (ec.RemoveEntity(e))
                        removed = true;
                return removed;
            }
        }

        /**
         * connects an entity to all other possible entities in entities
         */
        protected void ConnectToOthers(WorldEntity entity)
        {
            if (Entities.Count > 0 && !entity.IsFiller)
            {
                foreach (WorldEntity e in Entities)
                {
                    if (entity != e && !e.IsFiller)
                    {
                        foreach (Link lE in e.Links)
                            if (lE.ConnectionAvailable)
                            {
                                foreach (Link lEntity in entity.Links)
                                    if (lEntity.ConnectionAvailable && e.Contains(lEntity.AbsolutePosition-lE.RelativePositionRotated/2) && entity.Contains(lE.AbsolutePosition-lEntity.RelativePositionRotated/2)) //divided by 2 because of edges of links connecting to others
                                    {
                                        lE.ConnectTo(lEntity);
                                    }
                            }
                    }
                }
                
            }
        }
        
        /**
         * returns a list of sets of worldEntities connected to eachother
         */
        private List<HashSet<WorldEntity>> GetSetsOfEntities()
        {
            List<HashSet<WorldEntity>> sets = new List<HashSet<WorldEntity>>();
            //Entities.Sort((a, b) => a.Links.Count.CompareTo(a.Links.Count));
            foreach (WorldEntity e in Entities)
            {
                bool containsEntity = false;
                foreach (HashSet<WorldEntity> s in sets)
                    if (s.Contains(e))
                        containsEntity = true;
                if (!containsEntity)
                {
                    HashSet<WorldEntity> set = new HashSet<WorldEntity>();
                    set.Add(e);
                    GetConnectedEntities(e, set);
                    sets.Add(set);
                }
                
            }
            return sets;
        }

        /**
         * returns a set of all entities connected to each other, using WorldEntity e as starting point
         */
        private HashSet<WorldEntity> GetConnectedEntities(WorldEntity e, HashSet<WorldEntity> foundEntities)
        {
            foreach(Link l in e.Links)
                if(!l.ConnectionAvailable)
                    if (!foundEntities.Contains(l.connection.Entity))
                    {
                        foundEntities.Add(l.connection.Entity);
                        GetConnectedEntities(l.connection.Entity, foundEntities);
                    }
            return foundEntities;
        }

        protected void UpdateRadius() //TODO: Update this to make it more efficient, e.g. by having sorted list
        {
            if (Entities.Count == 1/* && SeperatedEntities.Count == 0*/)
            {
                if (Entities[0] != null)
                    Radius = Entities[0].Radius;
            }
            else if (Entities.Count > 1 /*|| SeperatedEntities.Count>0*/)
            {
                float largestDistance = 0;
                foreach (WorldEntity e in Entities)
                {
                    if (e.IsAlive) { 
                        float distance = Vector2.Distance(e.Position, Position) + e.Radius;
                        if (distance > largestDistance)
                            largestDistance = distance;
                    }
                }/*
                foreach(EntityController ec in SeperatedEntities)
                {
                    float distance = Vector2.Distance(ec.Position, Position) + ec.Radius;
                    if (distance > largestDistance)
                        largestDistance = distance;
                }
                */
                Radius = largestDistance;
            }
        }
        protected void UpdatePosition()
        {
            Vector2 sum = Vector2.Zero;
            float weight = 0;
            foreach (WorldEntity e in Entities)
            {
                if (e.IsAlive && !e.IsFiller)
                {
                    sum += e.Position * e.Mass;
                    weight += e.Mass;
                }
            }
            position = sum/weight;
            collisionDetector.Position = position;
            /*
            foreach (EntityController e in SeperatedEntities)
            {
                sum += e.Position * e.Mass;
                weight += e.Mass;
            }
            if (weight > 0)
                sum /= weight;
            position = sum;
            collisionDetector.Position = position;*/
        }
        public override void Shoot(GameTime gameTime)
        {
            foreach (WorldEntity e in Entities)
                if (e is Shooter gun)
                    gun.Shoot(gameTime);
        }
        public override void Collide(IControllable controllable) //OBS: ADD COLLISSION HANDLING SUPPORT FOR WORLDENTITIES NOT BELONGING TO ENTITY CONTROLLER
        {
            if (controllable is Controller c)
                foreach (IControllable iC in c.controllables)
                        Collide(iC);
            else if (controllable is EntityController eC)
            {
                if (CollidesWith(controllable))
                {
                    foreach (WorldEntity e in Entities)
                        foreach (WorldEntity eCE in eC.Entities)
                            if(!e.IsFiller && !eCE.IsFiller)
                                Collide(e, eCE);
                }/*
                foreach (Queue<Projectile> pList in projectiles)
                    foreach (Projectile p in pList)
                        foreach (WorldEntity eE in eC.entities) //OBS need adaption for new structure
                            Collide(p, eE);*/
            }
            else if (controllable is WorldEntity wE)
            {
                    foreach (WorldEntity e in Entities)
                        if (!e.IsFiller && !wE.IsFiller)
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
                TotalExteriorForce += Physics.CalculateOverlapRepulsion(e.Position - e.Velocity, eCE.Position - eCE.Velocity, e.Radius)*(e.Mass+eCE.Mass)/2;
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
                    TotalExteriorForce += Physics.CalculateOverlapRepulsion(e.Position - e.Velocity, eCE.Position, e.Radius) * (e.Mass + eCE.Mass) / 2;
                    TotalExteriorForce += 0.7f * Physics.CalculateOverlapRepulsion(Position, eCE.Position, e.Radius);
                }
                else if (Vector2.Dot(e.Velocity, -distanceBeforeMoving) > Vector2.Dot(eCE.Velocity, -distanceBeforeMoving) + distanceBeforeMoving.Length() && eCE.CollidesWithDuringMove(e))
                {
                    collides = true;
                    TotalExteriorForce += Physics.CalculateCollissionRepulsion(e.Position, eCE.Position - eCE.Velocity, e.Velocity * e.Mass, eCE.Velocity * e.Mass);
                    TotalExteriorForce += Physics.CalculateOverlapRepulsion(e.Position, eCE.Position - eCE.Velocity, e.Radius) * (e.Mass + eCE.Mass) / 2;
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
                        foreach(WorldEntity e in ec.Entities)
                            p.Collide(e);
        }

        public override void ApplyRepulsion(Entity otherEntity)
        {
            if (Entities.Count > 0)
            {
                if (Radius + otherEntity.Radius + REPULSIONDISTANCE > Vector2.Distance(Position, otherEntity.Position))
                {
                    if (otherEntity is EntityController otherEC)
                    {
                        foreach (WorldEntity e1 in Entities)
                            foreach (WorldEntity e2 in otherEC.Entities)
                            {
                                if (!e1.IsFiller && !e2.IsFiller)
                                    TotalExteriorForce += Mass / Entities.Count * CalculateGravitationalRepulsion(e1, e2);
                            }
                    }
                    else if (otherEntity is WorldEntity otherWE)
                    {
                        foreach (Entity e in Entities)
                        {
                            TotalExteriorForce += Mass / Entities.Count * CalculateGravitationalRepulsion(e, otherWE);
                        }
                    }
                }
            }
        }

        public override object Clone()
        {
            EntityController cNew = (EntityController)this.MemberwiseClone();
            cNew.collisionDetector = new CollidableCircle(Position, radius);
            cNew.projectiles = new List<Queue<Projectile>>();
            cNew.Entities = new List<WorldEntity>();
            cNew.Velocity = Vector2.Zero;
            foreach (WorldEntity e in Entities)
                cNew.AddEntity((WorldEntity)e.Clone());
            cNew.SeperatedEntities = new List<EntityController>();
            foreach (EntityController ec in SeperatedEntities)
                cNew.SeperatedEntities.Add((EntityController)ec.Clone());
            cNew.UpdatePosition();
            cNew.UpdateRadius();
            cNew.MoveAndRotateEntities();
            foreach (EntityController ec in cNew.SeperatedEntities)
            {
                ec.UpdatePosition();
                ec.UpdateRadius();
                ec.MoveAndRotateEntities();
                foreach (WorldEntity w in ec.Entities)
                    ec.ConnectToOthers(w);
            }
            
            return cNew;
        }

        public override void Update(GameTime gameTime)
        {
            MoveAndRotateEntities();
            Vector2 previousPosition = Position;
            base.Update(gameTime);
            foreach (WorldEntity e in Entities)
            {
                e.Position -= Position - previousPosition;
                e.Velocity += Position - previousPosition;
                e.Update(gameTime);
            }
                
            foreach (EntityController ec in SeperatedEntities)
                ec.Update(gameTime);
        }

        protected void RemoveEmptyControllers()
        {
            List<EntityController> toBeRemoved = new List<EntityController>();
            foreach (EntityController ec in SeperatedEntities)
                if (ec.Entities.Count == 0)
                    toBeRemoved.Add(ec);
            foreach (EntityController ec in toBeRemoved)
                SeperatedEntities.Remove(ec);
        }

        private void MoveAndRotateEntities()
        {
            float dRotation = oldRotation - Rotation;
            foreach (WorldEntity e in Entities)
            {
                Vector2 relativePosition = e.Position - Position;
                Vector2 newRelativePosition = Vector2.Transform(relativePosition, Matrix.CreateRotationZ(-dRotation));
                e.Velocity = newRelativePosition - relativePosition;
                e.Rotation = Rotation;

            }
            oldRotation = rotation;
        }
        public override bool CollidesWith(IIntersectable c) //NO CHECK DONE, JUST COPY PASTE
        {
            if (c is Controller)
                return collisionDetector.CollidesWith(((Controller)c).collisionDetector);
            else if (c is EntityController)
                return collisionDetector.CollidesWith(((EntityController)c).collisionDetector);
            if (c is WorldEntity && Vector2.Distance(c.Position, Position) < Radius/*collisionDetector.CollidesWith(((WorldEntity)c).collisionDetector*/)
                foreach (WorldEntity e in Entities)
                    if (e.CollidesWith((WorldEntity)c))
                        return true;
            return false;
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            foreach (WorldEntity e in Entities)
                e.Draw(spritebatch);
            foreach (EntityController e in SeperatedEntities)
                e.Draw(spritebatch);
        }

        public override bool Contains(Vector2 point)
        {
            foreach (WorldEntity e in Entities)
                if (e.Contains(point))
                    return true;
            /*foreach (EntityController ec in SeperatedEntities)
                if (ec.Contains(point))
                    return true;*/
            return false;
        }

        public void AddAvailableLinkDisplays()
        {
            List<WorldEntity> tempEntities = new List<WorldEntity>();
            foreach (WorldEntity e in Entities)
            {
                if (!e.IsFiller) { 
                    List<WorldEntity> fillers = e.FillEmptyLinks();
                    foreach (WorldEntity ee in fillers)
                        if(!Entities.Contains(ee))
                            tempEntities.Add(ee);
                }
            }
            foreach (WorldEntity eT in tempEntities)
            {
                bool overlaps = false;
                foreach (WorldEntity eE in Entities)
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
            /*
            
            foreach (WorldEntity e in entities)
            {
                foreach (WorldEntity ee in e.FillerEntities)
                    tempEntities.Add(ee);
            }
            foreach (WorldEntity e in tempEntities)
                entities.Remove(e);
            foreach(WorldEntity e in entities)
                e.ClearEmptyLinks();*/
            foreach (WorldEntity e in Entities)
            {
                if (e.IsFiller)
                {
                    foreach (Link l in e.Links)
                        l.SeverConnection();
                    tempEntities.Add(e);
                }
            }
            foreach (WorldEntity e in tempEntities)
                Entities.Remove(e);
            foreach (EntityController ec in SeperatedEntities)
                ec.ClearAvailableLinks();
            UpdateRadius();
        }
        public bool ReplaceEntity(WorldEntity eOld, WorldEntity eNew)
        {
            if (Entities.Contains(eOld))
            {
                eNew.ConnectTo(eOld.Links[0].connection.Entity, eOld.Links[0].connection);
                Entities.Remove(eOld);
                eOld.Links[0].SeverConnection();
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
            foreach (WorldEntity e in Entities)
                if (e.ControllableContainingInSpace(position, transform) != null)
                    return e;
            /*foreach (EntityController ec in SeperatedEntities)
                return ec.ControllableContainingInSpace(position, transform);*/
            return null;
        }
        #endregion
    }
}
