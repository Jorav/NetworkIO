using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.menu.controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.menu.states
{
    public abstract class MenuState : State
    {
        protected List<IComponent> components;

        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, Input input) : base(game, graphicsDevice, content, input)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            foreach (IComponent component in components)
                component.Draw(spriteBatch);
            spriteBatch.End();
        }
        
        public override void Update(GameTime gameTime)
        {
            foreach (IComponent component in components)
                component.Update(gameTime);
        }

        public override void PostUpdate()
        {
            //Remove sprites if they're not needed
            //throw new NotImplementedException();
        }

        
    }
}
