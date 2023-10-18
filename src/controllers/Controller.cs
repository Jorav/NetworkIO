using Microsoft.Xna.Framework;
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
    public class Controller : IComponent, IController
    {
        protected List<IControllable> controllables;
        public List<IControllable> Controllables { get { return controllables; } set { SetControllables(value); } }
        public CollidableCircle collisionDetector;
        protected float collissionOffset = 100; //TODO make this depend on velocity + other things?
        public float Radius { get { return radius; } protected set { radius = value; collisionDetector.Radius = value; } }
        protected float radius;
        public virtual Vector2 Position { get { return position; }
            set 
            {
                Vector2 posChange = value - Position;
                foreach (IControllable c in Controllables)
                    c.Position += posChange;
                position = value;
                collisionDetector.Position = value; 
            } 
        }

        public float Mass
        {
            get
            {
                float sum = 0;
                foreach (IControllable c in Controllables)
                    sum += c.Mass;
                return sum;
            }
        }

        public IController Manager { get; set; }

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
                List<IControllable> oldControllables = Controllables;
                controllables = new List<IControllable>();
                foreach (IControllable c in newControllables)
                    AddControllable(c);
                if (Controllables.Count == 0)
                {
                    Controllables = oldControllables;
                }
            }
        }
        public void AddControllable(IControllable c)
        {
            if (controllables == null)
            {
                controllables = new List<IControllable>();
                c.Position = Position;
            }
                
            if (c != null)
            {
                controllables.Add(c);
                UpdatePosition();
                UpdateRadius();
                c.Manager = this;
            }
        }

        public void RotateTo(Vector2 position)
        {
            foreach (IControllable c in Controllables)
                c.RotateTo(position);
        }

        public virtual void Update(GameTime gameTime)
        {
            RemoveEmptyControllers();
            UpdateControllable(gameTime);
            UpdatePosition();
            UpdateRadius();
            ApplyInternalGravity();
            ApplyInternalRepulsion();
            InternalCollission();
        }

        protected void RemoveEmptyControllers()
        {
            List<IControllable> toBeRemoved = new List<IControllable>();
            foreach (IControllable c in Controllables)
                if (c is WorldEntity we && !we.IsAlive)
                    toBeRemoved.Add(we);
                else if (c is EntityController ec && ec.Controllables.Count == 0)
                    toBeRemoved.Add(ec);
                else if (c is Controller cc && Controllables.Count == 0)
                    toBeRemoved.Add(cc);
            foreach (IControllable c in toBeRemoved)
                Remove(c);
        }

        protected virtual void AddSeperatedEntities()
        {
            List<EntityController> seperatedEntities = new List<EntityController>();
            foreach (IControllable c in Controllables)
                if (c is EntityController ec)
                    foreach (EntityController ecSeperated in ec.SeperatedEntities)
                    {
                        seperatedEntities.Add(ecSeperated);
                    }
            foreach(EntityController ec in seperatedEntities)
            {
                AddControllable(ec);
            }
            foreach (IControllable c in Controllables)
                if (c is EntityController ec)
                    ec.SeperatedEntities.Clear();
        }

        protected void InternalCollission()
        {
            foreach (IControllable c1 in Controllables)
                foreach (IControllable c2 in Controllables)
                    if (c1 != c2)
                        c1.Collide(c2);
        }

        private void UpdateControllable(GameTime gameTime)
        {
            foreach (IControllable c in Controllables)
                c.Update(gameTime);
        }

        //TODO: make this work 
        protected void UpdateRadius() //TODO: Update this to make it more efficient, e.g. by having sorted list, TODO: only allow IsCollidable to affect this?
        {
            if (Controllables.Count == 1)
            {
                if (Controllables[0] != null)
                    Radius = Controllables[0].Radius;
            }
            else if (Controllables.Count > 1)
            {
                float largestDistance = 0;
                foreach (IControllable c in Controllables)
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
            float nr = 1;
            float distance = 0;
            float mass = 0;
            foreach (IControllable c in Controllables)
            {
                distance += (Vector2.Distance(c.Position, Position) + c.Radius)*c.Mass;
                //nr += 1;
                mass += c.Mass;
            }
            if(mass != 0)
                return distance / nr/mass;
            return 1;
        }
        protected void ApplyInternalGravity()
        {
            Vector2 distanceFromController;
            foreach (IControllable c1 in Controllables)
            {
                distanceFromController = Position - c1.Position;
                if (distanceFromController.Length() > c1.Radius)
                    c1.Accelerate(Vector2.Normalize(Position - c1.Position), (float)Math.Pow(((distanceFromController.Length()-c1.Radius)/AverageDistance()) / 2*c1.Mass,2));
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

        protected void UpdatePosition() //TODO: only allow IsCollidable to affect this?
        {
            Vector2 sum = Vector2.Zero;
            float weight = 0;
            foreach (IControllable c in Controllables) 
            {
                weight += c.Mass;
                sum += c.Position*c.Mass;
            }
            if(weight > 0)
            {
                position = sum / (weight);
                collisionDetector.Position = position;
            }
                
                
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (IControllable c in Controllables)
                c.Draw(sb);
        }

        public virtual void Collide(IControllable collidable) // OBS - THIS NEEDS TO BE ADAPTED FOR ICOLLIDABLE
        {
            if (CollidesWith(collidable))//TODO(lowprio): Add predicitive collision e.g. by calculating many steps (make extended collisionobject starting from before calculation and ending where it ended)
                foreach (IControllable c in Controllables)
                    c.Collide(collidable);
            CollideProjectiles(collidable);
        }

        private void CollideProjectiles(IControllable collidable)
        {
            foreach (IControllable c in Controllables)
                if (c is Controller cc)
                    cc.CollideProjectiles(collidable);
                else if (c is EntityController ec)
                    ec.CollideProjectiles(collidable);
        }

        public bool CollidesWith(IIntersectable c)
        {
            if(c is Controller)
                return collisionDetector.CollidesWith(((Controller)c).collisionDetector);
            else if (c is EntityController)
                return collisionDetector.CollidesWith(((EntityController)c).collisionDetector);
            if (c is WorldEntity && collisionDetector.CollidesWith(((WorldEntity)c).collisionDetector))
                foreach (WorldEntity e in Controllables)
                    if (e.CollidesWith((WorldEntity)c))
                        return true;
            return false;
        }

        public void Accelerate(Vector2 directionalVector, float thrust)
        {
            foreach (IControllable c in Controllables)
                c.Accelerate(directionalVector, thrust);
        }
        public void Accelerate(Vector2 directionalVector)
        {
            foreach (IControllable c in Controllables)
                c.Accelerate(directionalVector);
        }
        public virtual object Clone()
        {
            Controller cNew = (Controller)this.MemberwiseClone();
            cNew.controllables = new List<IControllable>();
            foreach (IControllable c in Controllables)
                cNew.AddControllable((IControllable)c.Clone());
            cNew.collisionDetector = new CollidableCircle(Position, radius);
            return cNew;
        }

        public void Shoot(GameTime gameTime)
        {
            foreach (IControllable c in Controllables)
                c.Shoot(gameTime);
        }

        protected virtual void GiveOrders() { }

        public IControllable ControllableContainingInSpace(Vector2 position, Matrix transform)
        {
            IControllable controllable;
            foreach(IControllable c in Controllables)
            {
                controllable = c.ControllableContainingInSpace(position, transform);
                if (controllable != null)
                    return controllable;
            }
            return null;
        }
        public static String GetName()
        {
            return "No controller";
        }

        public bool Remove(IControllable c)
        {
            c.Manager = null;
            return Controllables.Remove(c);
        }
    }
}
     