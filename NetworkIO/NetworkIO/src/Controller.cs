using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    class Controller
    {
        public List<Entity> entities;

        public Controller(List<Entity> entities)
        {
            this.entities = entities;
        }
        public virtual void Update(GameTime gameTime)
        {
            foreach (Entity e in entities)
            {
                e.Move(gameTime);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Entity e in entities)
                e.Draw(sb);
        }
    }
}
