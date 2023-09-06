using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.menu.states.menu_states
{
    public abstract class BuildState : MenuState
    {
        protected GameState gameState;
        public MenuController menuController;
        protected Controller controllerEdited;
        public BuildState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GameState gameState, Input input, Controller controllerEdited) : base(game, graphicsDevice, content, input)
        {
            this.controllerEdited = controllerEdited;
            this.menuController = new MenuController(CopyEntitiesFromController(controllerEdited));
            this.gameState = gameState;
            gameState.Player.actionsLocked = true;
        }

        protected List<ICollidable> CopyEntitiesFromController(Controller controller)
        {
            List<ICollidable> collidables = new List<ICollidable>();
            foreach (ICollidable c in controller.collidables)
                collidables.Add((ICollidable)c.Clone());
            return collidables;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            gameState.RunGame(gameTime);
            menuController.Update(gameTime);
            if (input.PauseClicked)
                game.ChangeState(new PauseState(game, graphicsDevice, content, this, input));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            gameState.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);
            spriteBatch.Begin(transformMatrix: menuController.Camera.Transform, samplerState: SamplerState.PointClamp);
            menuController.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
