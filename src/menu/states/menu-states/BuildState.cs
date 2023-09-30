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
        protected State previousState;
        public MenuController menuController;
        protected Controller controllerEdited;
        protected bool buildMode;
        public int previousScrollValue;
        public int currentScrollValue;
        private readonly Sprite overlay;
        public BuildState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, State previousState, Input input, Controller controllerEdited) : base(game, graphicsDevice, content, input)
        {
            this.controllerEdited = controllerEdited;
            this.menuController = new MenuController(CopyEntitiesFromController(controllerEdited), input);
            this.previousState = previousState;
            if(previousState is GameState gameState)
                gameState.Player.actionsLocked = true;
            menuController.Camera.InBuildScreen = true;
            menuController.Camera.AutoAdjustZoom = true;
            currentScrollValue = input.ScrollValue;
            overlay = new Sprite(content.Load<Texture2D>("background/backgroundWhite"));
            overlay.Scale = overlay.Height / Game1.ScreenHeight;
            overlay.Position = new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
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
            menuController.Update(gameTime);
            if(previousState is GameState gameS)
                gameS.RunGame(gameTime);
            if (input.PauseClicked)
                game.ChangeState(new PauseState(game, graphicsDevice, content, this, input));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            previousState.Draw(gameTime, spriteBatch);
            spriteBatch.Begin();
            overlay.Draw(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(transformMatrix: menuController.Camera.Transform, samplerState: SamplerState.PointClamp);
            menuController.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime, spriteBatch);
        }
    }
}
