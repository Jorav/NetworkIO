using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NetworkIO.src.menu.controls;
using NetworkIO.src.menu.states.game_states;
using NetworkIO.src.menu.states.menu_states;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.menu.states
{
    class PauseState : MenuState
    {
        GameState gameState;
        public PauseState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GameState gameState) : base(game, graphicsDevice, content)
        {
            this.gameState = gameState;
            Texture2D buttonTexture = content.Load<Texture2D>("controls/Button");
            SpriteFont buttonFont = content.Load<SpriteFont>("fonts/Font");
            Sprite background = new Sprite(content.Load<Texture2D>("background/backgroundGray"));
            background.Scale = background.Height/ Game1.ScreenHeight;
            background.Position = new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);

            Button continueButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(300, 150), //or preferably center
                Text = "Continue",
            };
            continueButton.Click += ContinueButton_Click;

            Button newGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(300, 200), //or preferably center
                Text = "New Game",
            };
            newGameButton.Click += NewGameButton_Click;

            Button loadGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(300, 250), //or preferably center
                Text = "Load Game",
            };
            loadGameButton.Click += LoadGameButton_Click;

            Button mainMenuButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(300, 300), //or preferably center
                Text = "Main Menu",
            };
            mainMenuButton.Click += MainMenuButton_Click;

            Button quitGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(300, 350), //or preferably center
                Text = "Quit Game",
            };
            quitGameButton.Click += QuitGameButton_Click;

            components = new List<Component>()
            {
                background,
                continueButton,
                newGameButton,
                loadGameButton,
                mainMenuButton,
                quitGameButton,
            };
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            gameState.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (gameState.Player.PauseClicked)
                game.ChangeState(gameState);
        }
        private void ContinueButton_Click(object sender, EventArgs e)
        {
            game.ChangeState(gameState);
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            game.ChangeState(new TestState(game, graphicsDevice, content));
        }

        private void LoadGameButton_Click(object sender, EventArgs e)
        {
            //load game state from earlier
            throw new NotImplementedException();
        }

        private void MainMenuButton_Click(object sender, EventArgs e)
        {
            game.ChangeState(new MainMenu(game, graphicsDevice, content));
        }

        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            game.Exit();
        }
    }
}
