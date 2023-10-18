using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.entities.hulls
{
    public abstract class Composite : WorldEntity
    {
        protected WorldEntity[] components;
        protected int capacity;

        public Composite(Sprite sprite, Vector2 position, EntityController controller = null, int capacity = 4) : base(sprite, position)
        {
            this.capacity = capacity;
            if (controller == null)
                this.Manager = new EntityController(position, this);
            else
                this.Manager = controller;
            components = new WorldEntity[capacity];
        }
        /**
        public void AddEntity(Entity e, int pos) // TODO: add support for several hull parts?
        {
            if (pos >= 0 && pos < components.Length)
            {
                components[pos] = e;
            }
        }*/

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            /*
            foreach (WorldEntity e in components)
                if(e!=null)
                    e.Draw(sb);*/
            base.Draw(sb);
        }
    }
}
