using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class MenuController : CollidablesController
    {
        public Camera Camera { get; private set; }
        public MenuController(List<ICollidable> collidables) : base(collidables)
        {
            Camera = new Camera(this, true);
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
            foreach (Entity e in collidables)
                if (e.ContainsInSpace(Mouse.GetState().Position.ToVector2(), Camera.Transform)) //if (c is EntityController ec && ec.ContainsInSpace(Mouse.GetState().Position.ToVector2(), Camera.Transform))
                    return e;
            return null;
        }
    }
}
