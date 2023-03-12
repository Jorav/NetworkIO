﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.menu
{
    public interface Component
    {
        public abstract void Draw(SpriteBatch spritebatch);

        public abstract void Update(GameTime gameTime);
    }
}
