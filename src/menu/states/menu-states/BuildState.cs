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
        protected bool buildMode;
        public int previousScrollValue;
        public int currentScrollValue;
        public BuildState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GameState gameState, Input input, Controller controllerEdited) : base(game, graphicsDevice, content, input)
        {
            this.controllerEdited = controllerEdited;
            this.menuController = new MenuController(CopyEntitiesFromController(controllerEdited), input);
            this.gameState = gameState;
            gameState.Player.actionsLocked = true;
            menuController.Camera.InBuildScreen = true;
            menuController.Camera.AutoAdjustZoom = true;
            currentScrollValue = input.ScrollValue;
        }

        protected List<IControllable> CopyEntitiesFromController(Controller controller)
        {
            List<IControllable> collidables = new List<IControllable>();
            foreach (IControllable c in controller.controllables)
                collidables.Add((IControllable)c.Clone());
            return collidables;
        }

        public override void Update(GameTime gameTime)
        {
            previousScrollValue = currentScrollValue;
            currentScrollValue = input.ScrollValue;
            if (previousScrollValue - currentScrollValue != 0)
            {
                menuController.Camera.Zoom /= (float)Math.Pow(0.999, (currentScrollValue - previousScrollValue));
                menuController.Camera.AutoAdjustZoom = false;
            }
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
