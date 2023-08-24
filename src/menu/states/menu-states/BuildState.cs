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
        public MenuController controller;
        public BuildState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GameState gameState, Input input, Controller controller) : base(game, graphicsDevice, content, input)
        {
            this.gameState = gameState;
            List<Entity> entities = new List<Entity>();
            foreach (Entity e in controller.entities)
                entities.Add((Entity)e.Clone());
            this.controller = new MenuController(entities);
            this.controller.MoveTo(new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2));//or preferably center

            //gameState.Player.Camera.InBuildScreen = true;
            gameState.Player.actionsLocked = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            gameState.RunGame(gameTime);
            controller.Update(gameTime);
            if (input.PauseClicked)
                game.ChangeState(new PauseState(game, graphicsDevice, content, this, input));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            gameState.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);
            spriteBatch.Begin(transformMatrix: controller.Camera.Transform, samplerState: SamplerState.PointClamp);
            controller.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
