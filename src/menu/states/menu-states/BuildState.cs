using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
        Controller playerCopy;
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
            playerCopy = (Controller)gameState.Player.Clone();
            playerCopy.MoveTo(new Vector2(750, 450));


            Sprite background = new Sprite(content.Load<Texture2D>("background/backgroundWhite"));
            background.Scale = background.Height / Game1.ScreenHeight;
            background.Position = new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
            Texture2D buttonTexture = content.Load<Texture2D>("controls/Button");
            SpriteFont buttonFont = content.Load<SpriteFont>("fonts/Font");
            Button addEntityButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(1500, 900), //or preferably center
                Text = "Add Entity",
            };
            addEntityButton.Click += AddEntityButton_Click;

            components = new List<Component>()
            {                
                background,
                playerCopy,
                addEntityButton,
            };
        }

        private void AddEntityButton_Click(object sender, EventArgs e)
        {
            playerCopy.entities.Add(EntityFactory.Create(playerCopy.Position, IDs.SHOOTER));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //playerCopy.Update(gameTime);
            if (gameState.Player.BuildClicked)
            {
                gameState.Player.entities = playerCopy.entities;
                gameState.Player.MoveTo(gameState.Player.Position);
                game.ChangeState(gameState);
            }
                
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            gameState.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);
            //playerCopy.Draw(spriteBatch);

        }
    }
}