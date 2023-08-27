using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.menu
{
    public interface IComponent
    {
        public abstract void Draw(SpriteBatch spritebatch);

        public abstract void Update(GameTime gameTime);
    }
}
