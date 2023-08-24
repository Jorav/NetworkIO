using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class MenuController : Controller
    {

        public MenuController(List<Entity> entities) : base(entities)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /**
         * returns the entity clicked by player, null if no entity was clicked
         */
        public Entity EntityClicked()
        {
            //foreach (Entity e in entities)
                //if (e.Contains())
                    //return e;
            return null;
        }
    }
}
