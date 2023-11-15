using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.collidables;
using NetworkIO.src.collission_detectors;
using NetworkIO.src.controllers;
using NetworkIO.src.entities;
using NetworkIO.src.factories;
using NetworkIO.src.menu;
using NetworkIO.src.movable;
using NetworkIO.src.utility;
using System;
using System.Collections.Generic;

namespace NetworkIO.src
{
    public class WorldEntity : Entity, IIntersectable, IComponent
    {
        #region Properties
        protected Sprite sprite = null;
        public bool IsVisible { get { return sprite.isVisible; } set { sprite.isVisible = value; } }
        public override Color Color { set { sprite.Color = value; } }
        public CollisionDetector collisionDetector;
        public CollisionDetector oldCollisionDetector;
        public override Vector2 Position { get { return position; } 
            set
            { 
                position = value;
                sprite.Position = value;
                oldCollisionDetector.Position = collisionDetector.Position;
                collisionDetector.Position = value;
            } 
        }
        public override float Rotation { get { return rotation; } 
            set 
            { 
                rotation = value + internalRotation; 
                sprite.Rotation = value+internalRotation; 
                oldCollisionDetector.Rotation = collisionDetector.Rotation; 
                collisionDetector.Rotation = value+internalRotation; 
            } 
        }
        public Vector2 Origin { get { return origin; } 
            set 
            { 
                origin = value; 
                sprite.Origin = value;
                //oldCollisionDetector.Origin = collisionDetector.Origin;
                //collisionDetector.Origin = value; 
            } 
        }
        protected Vector2 origin;
        public float Width { get { return sprite.Width; } }
        public float Height { get { return sprite.Height; } }
        public float Health { get { return health; } 
            set 
            {
                if (health > value)
                    sprite.DamageEffect();
                health = value; 
                if(value<=0) 
                    Die();
            } 
        }
        protected float health;
        public bool IsAlive { get { return health > 0; } }
        public override float Radius { get { return collisionDetector.Radius; } }
        public List<Link> Links { get; private set; }
        private float internalRotation;
        public bool IsFiller { get; set; }
        public float Scale { get { return sprite.Scale; } set { sprite.Scale = value; collisionDetector.Scale = value; oldCollisionDetector.Scale = value; foreach (Link l in Links) l.Scale = value;/*add collisionDetector scale in the future*/ } }
        #endregion
        public WorldEntity(Sprite sprite, Vector2 position, EntityController entityController = null, float rotation = 0, float mass = 1, float thrust = 1, float friction = 0.1f, float health = 10, bool isVisible = true, bool isCollidable = true,  float elasticity = 1) : base(position, rotation, mass, thrust, friction)
        {
            this.sprite = sprite;
            CreateCollisionDetectors(sprite, position, rotation);
            Position = position;
            Elasticity = elasticity;
            Health = health;
            IsVisible = isVisible;
            IsCollidable = isCollidable;
            Origin = new Vector2(Width / 2, Height / 2);
            Links = new List<Link>();
            AddLinks();
            if (entityController == null)
                this.Manager = new EntityController(position, this);
            else
                this.Manager = entityController;
        }

        protected virtual void CreateCollisionDetectors(Sprite sprite, Vector2 position, float rotation)
        {
            collisionDetector = CollidableFactory.CreateRectangular(position, rotation, sprite.Width, sprite.Height);
            oldCollisionDetector = CollidableFactory.CreateRectangular(position, rotation, sprite.Width, sprite.Height);
        }
        #region Methods
        protected virtual void AddLinks()
        {
            if (Links.Count > 0)
                Links.Clear();
            Links.Add(new Link(new Vector2(-Width/2, 0), this));
        }
        public override void Draw(SpriteBatch sb)
        {
            sprite.Draw(sb);
        }
        public virtual void Die()
        {
            TotalExteriorForce = Vector2.Zero;
            Velocity = Vector2.Zero;
            IsVisible = false;
            IsCollidable = false;
            foreach(Link l in Links)
            {
                l.SeverConnection();
            }
        }

        public override void Update(GameTime gameTime) //OBS Ska vara en funktion i thruster
        {
            base.Update(gameTime);
            if (Health < 0)
                Die();
        }

        public override bool Contains(Vector2 point)
        {
            return IsCollidable && collisionDetector.Contains(point);
        }

        public override IControllable ControllableContainingInSpace(Vector2 position, Matrix transform)
        {
            if (IsCollidable && collisionDetector.ContainsInSpace(position, transform))
                return this;
            return null;
        }

        public bool CollidesWith(WorldEntity e)
        {
            if (IsCollidable && e.IsCollidable)
            {
                if (collisionDetector.CollidesWith(e.collisionDetector))
                    return true;
            }
            return false;
        }
        public bool CollidesWithDuringMove(WorldEntity e)
        {
            bool collides = false;
            collisionDetector.StretchTo(oldCollisionDetector);
            collides = collisionDetector.CollidesWithStretch(e.collisionDetector) && IsCollidable && e.IsCollidable;
            collisionDetector.StopStretch();
            return collides;
        }

        public override void Collide(IControllable c)
        {
            if (c is WorldEntity e)
            {
                //collision direct
                if (CollidesWith(e))
                {
                    TotalExteriorForce += Physics.CalculateCollissionRepulsion(Position, e.Position, Velocity * Mass, e.Velocity * e.Mass);
                    TotalExteriorForce += Physics.CalculateOverlapRepulsion(Position, e.Position, Radius) * (e.Mass + Mass) / 2;
                }
                else
                {
                    Vector2 distanceBeforeMoving = Position - Velocity - (e.Position - e.Velocity);
                    Vector2 distance = Position - e.Position;

                    //collision "from behind"
                    if (Vector2.Dot(e.Velocity, distanceBeforeMoving) > Vector2.Dot(Velocity, distanceBeforeMoving) + distanceBeforeMoving.Length() && e.CollidesWithDuringMove(this))//if they move
                    {
                        TotalExteriorForce += Physics.CalculateCollissionRepulsion(Position - Velocity, e.Position, Velocity*Mass, e.Velocity*e.Mass);
                        TotalExteriorForce += Physics.CalculateOverlapRepulsion(Position - Velocity, e.Position, Radius) * (e.Mass + Mass) / 2;
                    }

                    //collision "passing through"
                    else if (Vector2.Dot(Velocity, -distanceBeforeMoving) > Vector2.Dot(e.Velocity, -distanceBeforeMoving) + distanceBeforeMoving.Length() && e.CollidesWithDuringMove(this))
                    {
                        TotalExteriorForce += Physics.CalculateCollissionRepulsion(Position, e.Position - e.Velocity, Velocity * Mass, e.Velocity * e.Mass);
                        TotalExteriorForce += Physics.CalculateOverlapRepulsion(Position, e.Position - e.Velocity, Radius) * (e.Mass + Mass) / 2;
                    }
                }
            }
            else if (c is Controller)
                foreach (IControllable cc in ((Controller)c).Controllables)
                    Collide(cc);
            else if (c is EntityController)
                foreach (IControllable cc in ((EntityController)c).Controllables)
                    Collide(cc);
        }

        public virtual void HandleCollision(WorldEntity eOther, bool passesThroughFromBack = false, bool passesThroughFromFront = false)
        {
            if (!(Manager != null && Manager is EntityController))
            {
                if (passesThroughFromBack)
                {
                    TotalExteriorForce += Physics.CalculateOverlapRepulsion(Position - Velocity, eOther.Position, Radius) * (eOther.Mass + Mass) / 2;
                }
                else if (passesThroughFromFront)
                {
                    TotalExteriorForce += Physics.CalculateCollissionRepulsion(Position, eOther.Position - eOther.Velocity, Velocity * Mass, eOther.Velocity * eOther.Mass);
                    TotalExteriorForce += Physics.CalculateOverlapRepulsion(Position, eOther.Position - eOther.Velocity, Radius) * (eOther.Mass + Mass) / 2;
                }
                else
                {
                    TotalExteriorForce += Physics.CalculateCollissionRepulsion(Position, eOther.Position, Velocity * Mass, eOther.Velocity * eOther.Mass);
                    TotalExteriorForce += Physics.CalculateOverlapRepulsion(Position, eOther.Position, Radius) * (eOther.Mass + Mass) / 2;
                }
            }
        }

        public override object Clone()
        {
            WorldEntity eNew = (WorldEntity)this.MemberwiseClone();
            eNew.sprite = (Sprite)this.sprite.Clone();
            eNew.collisionDetector = new CollidableRectangle(position, rotation, (int)Width, (int)Height);
            eNew.Velocity = Vector2.Zero;
            eNew.TotalExteriorForce = Vector2.Zero;
            eNew.Links = new List<Link>();
            eNew.AddLinks();
            return eNew;
        }

        public override bool CollidesWith(IIntersectable c)
        {
            if(c is WorldEntity ce)
                return IsCollidable && ce.IsCollidable && collisionDetector.CollidesWith(ce.collisionDetector);
            if (c is Controller cc)
                return cc.CollidesWith(this);
            return false;
        }

        public void ConnectTo(WorldEntity eConnectedTo, Link lConnectedTo)
        {
            if (Links.Count > 0 && Links[0] != null) {
                lConnectedTo.SeverConnection();
                internalRotation = Links[0].ConnectTo(lConnectedTo);
                Rotation = eConnectedTo.Rotation-eConnectedTo.internalRotation;
                Position = lConnectedTo.ConnectionPosition;
            }
        }
        public List<WorldEntity> FillEmptyLinks()
        {
            List<WorldEntity> entities = new List<WorldEntity>();
            IDs fillerType = IDs.EMPTY_LINK;
            foreach (Link l in Links)
                if (l.ConnectionAvailable)
                {
                    WorldEntity e = EntityFactory.Create(position, fillerType);
                    e.ConnectTo(this, l);
                    e.IsFiller = true;
                    entities.Add(e);
                }
            return entities;
        }

        public void SeverConnection(WorldEntity e)
        {
            foreach(Link l in Links)
                if(l.ConnectionAvailable && l.connection.Entity == e)
                {
                    l.SeverConnection();
                }
        }
        #endregion

        public class Link
        {
            public WorldEntity Entity { get; private set; }
            public Link connection; //links to other entities
            public Vector2 RelativePosition { get; private set; } //position in relation to the entity it belongs to in an unrotated state
            public Vector2 RelativePositionRotated { get { return RelativePosition.Length() * Scale * new Vector2((float)Math.Cos(MathHelper.WrapAngle(LinkRotation + Entity.Rotation)), (float)Math.Sin(MathHelper.WrapAngle(LinkRotation + Entity.Rotation))); } }
            public Vector2 AbsolutePosition { get { return Entity.Position + RelativePositionRotated; } }
            public Vector2 ConnectionPosition { get { Vector2 dir = new Vector2((float)Math.Cos(MathHelper.WrapAngle(LinkRotation + Entity.Rotation)), (float)Math.Sin(MathHelper.WrapAngle(LinkRotation + Entity.Rotation))); if (!ConnectionAvailable) return Entity.Position + DistanceFromConnection * dir; else return Entity.Position + dir *RelativePosition.Length() * Scale* 2; } }
            public float Scale { get; set; }
            public float LinkRotation { get; set; } //rotation of link in relation to center of entity
            public float DistanceFromConnection { get { if (!ConnectionAvailable) return RelativePosition.Length()* Scale + connection.RelativePosition.Length()* connection.Scale; throw new Exception(); } }
            public bool ConnectionAvailable { get { return connection == null; } }

            public Link(Vector2 relativePosition, WorldEntity entity, Link connection = null)
            {
                this.Entity = entity;
                this.RelativePosition = relativePosition;
                this.connection = connection;
                if (relativePosition.Length() != 0)
                {
                    if (relativePosition.X >= 0)
                        LinkRotation = (float)Math.Atan(relativePosition.Y / relativePosition.X);
                    else
                        LinkRotation = (float)Math.Atan(relativePosition.Y / relativePosition.X) - MathHelper.ToRadians(180);
                }
                Scale = 1;
            }

            /**
             * returns the internal rotation of the entity it belongs to
             */
            public float ConnectTo(Link l)
            {
                if (!l.ConnectionAvailable)
                    l.SeverConnection();
                connection = l;
                l.connection = this;
                return MathHelper.WrapAngle(l.Entity.internalRotation + l.LinkRotation + LinkRotation + MathHelper.ToRadians(180)) ;
                //return MathHelper.ToRadians(180) - MathHelper.WrapAngle(-l.LinkRotation - LinkRotation);
            }

            public void SeverConnection()
            {
                if (!ConnectionAvailable)
                {
                    connection.connection = null;
                    connection = null;
                }
            }
            /*
            public object Clone()
            {
                Link lNew = (Link)this.MemberwiseClone();
            }*/
        }
    }
}
