﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.collidables;
using NetworkIO.src.controllers;
using NetworkIO.src.entities;
using NetworkIO.src.menu;
using NetworkIO.src.movable;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NetworkIO.src
{
    public class Controller : IComponent, IControllable
    {
        public List<IControllable> controllables { get; protected set; }
        public CollidableCircle collisionDetector;
        protected float collissionOffset = 100; //TODO make this depend on velocity + other things?
        public float Radius { get { return radius; } protected set { radius = value; collisionDetector.Radius = value; } }
        protected float radius;
        public Vector2 Position { get { return position; } set { position = value; collisionDetector.Position = value; } }

        public float Mass
        {
            get
            {
                float sum = 0;
                foreach (IControllable c in controllables)
                    sum += c.Mass;
                return sum;
            }
        }

        protected Vector2 position;

        public Controller(List<IControllable> controllables)
        {
            this.collisionDetector = new CollidableCircle(Position, Radius);
            SetControllables(controllables);
        }

        public Controller([OptionalAttribute] Vector2 position)
        {
            this.collisionDetector = new CollidableCircle(Position, Radius);
            if (position == null)
                position = Vector2.Zero;
            SetControllables(new List<IControllable>() { new EntityController(position) });
        }
        public virtual void SetControllables(List<IControllable> newControllables)
        {
            if (newControllables != null)
            {
                List<IControllable> oldControllables = controllables;
                controllables = new List<IControllable>();
                foreach (IControllable c in newControllables)
                    AddControllable(c);
                if (controllables.Count == 0)
                {
                    controllables = oldControllables;
                }
            }
        }
        public void AddControllable(IControllable c)
        {
            if (c != null)
            {
                controllables.Add(c);
                UpdatePosition();
                UpdateRadius();
            }
        }

        public void RotateTo(Vector2 position)
        {
            foreach (IControllable c in controllables)
                c.RotateTo(position);
        }

        public void MoveTo(Vector2 newPosition) //OBS needs rework
        {
            UpdatePosition();
            Vector2 posChange = newPosition - Position;
            foreach (IControllable c in controllables)
                c.Position += posChange;
            Position = position + posChange;
        }

        public virtual void Update(GameTime gameTime)
        {
            UpdateControllable(gameTime);
            UpdatePosition();
            UpdateRadius();
            ApplyInternalGravity();
            ApplyInternalRepulsion();
            InternalCollission();
        }

        private void InternalCollission()
        {
            foreach (IControllable c1 in controllables)
                foreach (IControllable c2 in controllables)
                    if (c1 != c2)
                        c1.Collide(c2);
        }

        private void UpdateControllable(GameTime gameTime)
        {
            foreach (IControllable c in controllables)
                c.Update(gameTime);
        }

        //TODO: make this work 
        protected void UpdateRadius() //TODO: Update this to make it more efficient, e.g. by having sorted list, TODO: only allow IsCollidable to affect this?
        {
            if (controllables.Count == 1)
            {
                if (controllables[0] != null)
                    Radius = controllables[0].Radius;
            }
            else if (controllables.Count > 1)
            {
                float largestDistance = 0;
                foreach (IControllable c in controllables)
                {
                    float distance = Vector2.Distance(c.Position, Position)+c.Radius;
                    if (distance > largestDistance)
                        largestDistance = distance;
                }
                Radius = largestDistance;
            }
        }
        protected float AverageDistance()
        {
            float nr = 0;
            float distance = 0;
            foreach (IControllable c in controllables)
            {
                distance += (Vector2.Distance(c.Position, Position) + c.Radius);
                nr += 1;
            }
            if(nr != 0)
                return distance / nr;
            return 1;
        }
        protected void ApplyInternalGravity()
        {
            Vector2 distanceFromController;
            foreach (IControllable c1 in controllables)
            {
                distanceFromController = Position - c1.Position;
                if (distanceFromController.Length() != 0)
                    c1.Accelerate(Vector2.Normalize(Position - c1.Position), (distanceFromController.Length()/AverageDistance()) / 10);
            }
        }
        public void ApplyInternalRepulsion()
        {
            foreach (IControllable c1 in controllables)
            {
                foreach (IControllable c2 in controllables)//TODO: only allow IsCollidable to affect this?
                {
                    if (c1 != c2 && c1 is Entity e1 && c2 is Entity e2)
                        e1.ApplyRepulsion(e2);
                }
            }
        }

        protected void UpdatePosition() //TODO: only allow IsCollidable to affect this?
        {
            Vector2 sum = Vector2.Zero;
            float weight = 0;
            foreach (IControllable c in controllables) 
            {
                weight += c.Mass;
                sum += c.Position*c.Mass;
            }
            if(weight > 0)
                Position = sum / (weight);
                
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (IControllable c in controllables)
                c.Draw(sb);
        }

        public virtual void Collide(IControllable collidable) // OBS - THIS NEEDS TO BE ADAPTED FOR ICOLLIDABLE
        {
            if (CollidesWith(collidable))//TODO(lowprio): Add predicitive collision e.g. by calculating many steps (make extended collisionobject starting from before calculation and ending where it ended)
                foreach (IControllable c in controllables)
                    c.Collide(collidable);
        }

        public bool CollidesWith(IIntersectable c)
        {
            if(c is Controller)
                return collisionDetector.CollidesWith(((Controller)c).collisionDetector);
            else if (c is EntityController)
                return collisionDetector.CollidesWith(((EntityController)c).collisionDetector);
            if (c is WorldEntity && collisionDetector.CollidesWith(((WorldEntity)c).collisionDetector))
                foreach (WorldEntity e in controllables)
                    if (e.CollidesWith((WorldEntity)c))
                        return true;
            return false;
        }

        public void Accelerate(Vector2 directionalVector, float thrust)
        {
            foreach (IControllable c in controllables)
                c.Accelerate(directionalVector, thrust);
        }

        public virtual object Clone()
        {
            Controller cNew = (Controller)this.MemberwiseClone();
            cNew.controllables = new List<IControllable>();
            foreach (IControllable c in controllables)
                cNew.AddControllable((IControllable)c.Clone());
            cNew.collisionDetector = new CollidableCircle(Position, radius);
            return cNew;
        }

        public void Shoot(GameTime gameTime)
        {
            foreach (IControllable c in controllables)
                c.Shoot(gameTime);
        }

        protected virtual void GiveOrders() { }

        public IControllable ControllableContainingInSpace(Vector2 position, Matrix transform)
        {
            IControllable controllable;
            foreach(IControllable c in controllables)
            {
                controllable = c.ControllableContainingInSpace(position, transform);
                if (controllable != null)
                    return controllable;
            }
            return null;

        }
    }
}
     