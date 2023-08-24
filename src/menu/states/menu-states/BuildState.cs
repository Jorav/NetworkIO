using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.controllers;
using NetworkIO.src.factories;
using NetworkIO.src.menu.controls;
using NetworkIO.src.utility;
using System;
using System.Collections.Generic;

namespace NetworkIO.src.menu.states
{
    public class BuildState : MenuState
    {
        GameState gameState;
        MenuController controller;
        public BuildState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GameState gameState) : base(game, graphicsDevice, content)
        {
            this.gameState = gameState;
            /*
            List<Entity> playerEntities = new List<Entity>();
            gameState.Player.entities.ForEach((entity) =>
            {
                playerEntities.Add((Entity)entity.Clone());
            });
            playerCopy = new Controller(playerEntities);
            */
            
            
            List<Entity> entities = new List<Entity>();
            foreach (Entity e in gameState.Player.entities)
                entities.Add((Entity)e.Clone());
            controller = new MenuController(entities);
            controller.MoveTo(new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2));//or preferably center

            //gameState.Player.Camera.InBuildScreen = true;
            gameState.Player.inputLocked = true;

            Sprite background = new Sprite(content.Load<Texture2D>("background/backgroundWhite"));
            background.Scale = background.Height / Game1.ScreenHeight;
            background.Position = new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
            Texture2D buttonTexture = content.Load<Texture2D>("controls/Button");
            SpriteFont buttonFont = content.Load<SpriteFont>("fonts/Font");
            Button addEntityButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(Game1.ScreenWidth-buttonTexture.Width-100, Game1.ScreenHeight - buttonTexture.Height-100), //make this vary with Zoom
                Text = "Add Hull",
            };
            addEntityButton.Click += AddEntityButton_Click;

            components = new List<Component>()
            {                
                background,
                //controller,
                addEntityButton,
            };
        }

        private void AddEntityButton_Click(object sender, EventArgs e)
        {
            controller.entities.Add(EntityFactory.Create(controller.Position, IDs.SHOOTER));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            controller.Update(gameTime);
            gameState.RunGame(gameTime);
            //playerCopy.Update(gameTime);
            //if(gameState.Player.)
            if (gameState.Player.BuildClicked)
            {
                gameState.Player.SetEntities(controller.entities);
                gameState.Player.MoveTo(gameState.Player.Position);
                game.ChangeState(gameState);
                gameState.Player.Camera.InBuildScreen = false;
                gameState.Player.inputLocked = false;
            }  
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            gameState.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);
            spriteBatch.Begin(transformMatrix: controller.Camera.Transform);
            controller.Draw(spriteBatch);
            spriteBatch.End();
            //playerCopy.Draw(spriteBatch);

        }
    }
}