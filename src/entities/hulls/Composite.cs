using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.entities.hulls
{
    public abstract class Composite : Entity
    {
        protected Entity[] components;
        protected CompositeController compositeController;
        protected int capacity;

        public Composite(Sprite sprite, Vector2 position, CompositeController controller = null, int capacity = 4) : base(sprite, position)
        {
            this.capacity = capacity;
            if (controller == null)
                this.compositeController = new CompositeController(new List<Entity>() { this });
            else
                this.compositeController = controller;
            components = new Entity[capacity];
        }
        /**
        public void AddEntity(Entity e, int pos) // TODO: add support for several hull parts?
        {
            if (pos >= 0 && pos < components.Length)
            {
                components[pos] = e;
            }
        }*/

        public void AddEntity(Entity e, int pos)
        {
            compositeController.AddEntity(e);
            if (Links.Count > pos && e.Links.Count > 0)
                Links[pos].Connect(e.Links[0]);
            else
                throw new Exception("Cannot connect these links");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            foreach (Entity e in components)
                if(e!=null)
                    e.Draw(sb);
            base.Draw(sb);
        }
    }
}
