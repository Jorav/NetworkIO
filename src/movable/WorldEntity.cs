﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.collidables;
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
    public class WorldEntity : Entity, IComponent, IIntersectable
    {
        protected Sprite sprite = null;
        public CollidableRectangle collisionDetector;
        public EntityController EntityController { get; set; }
        public override Vector2 Position { get { return position; } set{ position = value; sprite.Position = value; collisionDetector.Position = value; } }
        public override float Rotation { get { return rotation; } set { rotation = value + internalRotation; sprite.Rotation = value+internalRotation; collisionDetector.Rotation = value+internalRotation; } }
        public Vector2 Origin { get { return origin; } set { origin = value; sprite.Origin = value; collisionDetector.Origin = value; } }
        protected Vector2 origin;
        public float Width { get { return sprite.Width; } }
        public float Height { get { return sprite.Height; } }
        public float Health { get { return health; } set { health = value; if(value<=0) Die(); } }
        protected float health;
        public override float Radius { get { return collisionDetector.Radius; } }
        public List<Link> Links { get; private set; }
        private float internalRotation;
        private List<WorldEntity> emptyLinks;

        public WorldEntity(Sprite sprite, Vector2 position, EntityController entityController = null, float rotation = 0, float mass = 1, float thrust = 1, float friction = 0.1f, float health = 100, bool isVisible = true, bool isCollidable = true,  float elasticity = 1) : base(position, rotation, mass, thrust, friction)
        {
            this.sprite = sprite;
            collisionDetector = (CollidableRectangle) CollidableFactory.CreateCollissionDetector(position, rotation, sprite.Width, sprite.Height);
            if (entityController == null)
                this.EntityController = new EntityController(position, this);
            else
                this.EntityController = entityController;
            
            Elasticity = elasticity;
            Health = health;
            IsVisible = isVisible;
            IsCollidable = isCollidable;
            Origin = new Vector2(Width / 2, Height / 2);
            Links = new List<Link>();
            AddLinks();
            emptyLinks = new List<WorldEntity>();
        }

        protected virtual void AddLinks()
        {
            if (Links.Count > 0)
                Links.Clear();
            Links.Add(new Link(new Vector2(-Width/2, 0)));
        }

        public override void Draw(SpriteBatch sb)
        {
            if (IsVisible)
            {
                foreach (WorldEntity e in emptyLinks)
                    e.Draw(sb);
                sprite.Draw(sb);
            }
            
        }
        public virtual void Die()
        {
            TotalExteriorForce = Vector2.Zero;
            Velocity = Vector2.Zero;
            IsVisible = false;
            IsCollidable = false;
        }

        public override void Update(GameTime gameTime) //OBS Ska vara en funktion i thruster
        {
            Velocity = Physics.CalculateVelocity(Position, Velocity, TotalExteriorForce, Mass, Friction);
            Position += Velocity;
            TotalExteriorForce = Vector2.Zero;
        }

        public override bool Contains(Vector2 point)
        {
            return IsCollidable && collisionDetector.Contains(point);
        }

        public override bool ContainsInSpace(Vector2 position, Matrix transform)
        {
            return IsCollidable && collisionDetector.ContainsInSpace(position, transform);
        }

        public bool CollidesWith(WorldEntity e)
        {
            return IsCollidable && e.IsCollidable && collisionDetector.CollidesWith(e.collisionDetector);
        }

        public override void Collide(IControllable c)
        {
            if (c is WorldEntity e)
            {
                //float r = Vector2.Distance(Position, e.Position);
                Vector2 directionalVector = Position - e.Position;
                directionalVector.Normalize();
                TotalExteriorForce += Physics.CalculateCollisionRepulsion(Position, Velocity, Mass, e.Position, e.Velocity, e.Mass, Math.Min(Math.Max(e.Width, e.Height), Math.Max(Width, Height)), Elasticity, e.Elasticity); //OBS: might break after changes
            }
            else if (c is Controller)
                foreach (IControllable cc in ((Controller)c).controllables)
                    Collide(cc);
            else if (c is EntityController)
                foreach (IControllable cc in ((EntityController)c).entities)
                    Collide(cc);
        }

        public override object Clone()
        {
            WorldEntity eNew = (WorldEntity)this.MemberwiseClone();
            eNew.sprite = (Sprite)this.sprite.Clone();
            eNew.collisionDetector = new CollidableRectangle(position, rotation, (int)Width, (int)Height);
            eNew.Velocity = Vector2.Zero;
            eNew.TotalExteriorForce = Vector2.Zero;
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
                internalRotation = Links[0].ConnectTo(lConnectedTo);
                Position = eConnectedTo.Position + lConnectedTo.DistanceFromConnection * 
                    new Vector2((float)Math.Cos(eConnectedTo.Rotation + lConnectedTo.LinkRotation), (float)Math.Sin(eConnectedTo.Rotation + lConnectedTo.LinkRotation));
                Rotation = eConnectedTo.Rotation;
            }
        }
        public void FillEmptyLinks()
        {
            IDs fillerType = IDs.EMPTY_LINK;
            foreach (Link l in Links)
                if (l.ConnectionAvailable())
                {
                    WorldEntity e = EntityFactory.Create(position, fillerType);
                    e.ConnectTo(this, l);
                    emptyLinks.Add(e);
                }
        }
        public void ClearAvailableLinks()
        {
            foreach (WorldEntity e in emptyLinks)
                foreach (Link l in e.Links) {
                    l.SeverConnection();
                }
            emptyLinks.Clear();
        }

        public class Link
        {
            private Link connection; //links to other entities
            private Vector2 relativePosition; //position in relation to the entity it belongs to in an unrotated state
            public float LinkRotation { get; private set; } //rotation of link in relation to center of entity
            public float DistanceFromConnection { get { if (!ConnectionAvailable()) return relativePosition.Length() + connection.relativePosition.Length(); return -1; } }

            public Link(Vector2 relativePosition, Link connection = null)
            {
                if (relativePosition.Length() == 0)
                    throw new ArgumentException();

                this.relativePosition = relativePosition;
                this.connection = connection;
                if (relativePosition.X >= 0)
                    LinkRotation = (float)Math.Atan(relativePosition.Y / relativePosition.X);
                else
                    LinkRotation = (float)Math.Atan(relativePosition.Y / relativePosition.X) - MathHelper.ToRadians(180);
            }

            /**
             * returns the internal rotation of the entity it belongs to
             */
            public float ConnectTo(Link l)
            {
                connection = l;
                l.connection = this;
                return MathHelper.ToRadians(180)- MathHelper.WrapAngle(- l.LinkRotation - LinkRotation);
            }

            public bool ConnectionAvailable()
            {
                return connection == null;
            }

            public void SeverConnection()
            {
                if (!ConnectionAvailable())
                {
                    connection.connection = null;
                    connection = null;
                }
            }
        }
    }
}