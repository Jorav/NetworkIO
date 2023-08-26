using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class MenuController : Controller
    {
        public Camera Camera { get; private set; }
        public MenuController(List<Entity> entities) : base(entities)
        {
            Camera = new Camera(this);
            Camera.InBuildScreen = true;
        }

        public override void Update(GameTime gameTime)
        {
            Camera.Update();
            base.Update(gameTime);
        }

        /**
         * returns the entity clicked by player, null if no entity was clicked
         */
        public Entity MouseOnEntity()
        {
            foreach (Entity e in entities)
                if (e.ContainsInSpace(Mouse.GetState().Position.ToVector2(), Camera.Transform))
                    return e;
            return null;
        }
    }
}
