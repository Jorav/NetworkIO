using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.entities
{
    public class Composite : Entity
    {
        //private List<Entity> entities;
        private Entity[] components;

        public Composite(Sprite sprite, Vector2 position) : base(sprite, position)
        {
            components = new Entity[4];
        }

        public void AddEntity(Entity e, int pos) // TODO: add support for several hull parts?
        {
            if (pos >= 0 && pos < components.Length)
            {
                components[pos] = e;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            for (int i=0; i < components.Length; i++)
            {
                if(components[i] != null) { 
                    components[i].Update(gameTime);
                    components[i].Rotation = rotation - i * (float)Math.PI / 2;
                    components[i].Position = Position + 
                        new Vector2((float)Math.Cos(components[i].Rotation) * Width / 2, (float)Math.Sin(components[i].Rotation) * Height / 2) + 
                        new Vector2((float)Math.Cos(components[i].Rotation) * components[i].Width / 2, (float)Math.Sin(components[i].Rotation) * components[i].Width / 2);


                }
            }
        }
        public override void Draw(SpriteBatch sb, Matrix parentMatrix)
        {
            foreach (Entity e in components)
                if(e!=null)
                    e.Draw(sb, parentMatrix);
            base.Draw(sb, parentMatrix);
        }
    }
}
