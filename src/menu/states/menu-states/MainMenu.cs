using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.menu.controls;
using NetworkIO.src.menu.states.game_states;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.menu.states.menu_states
{
    public class MainMenu : MenuState
    {
        public MainMenu(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, Input input) : base(game, graphicsDevice, content, input)
        {
            Texture2D buttonTexture = content.Load<Texture2D>("controls/Button");
            SpriteFont buttonFont = content.Load<SpriteFont>("fonts/Font");
            Sprite background = new Sprite(content.Load<Texture2D>("background/background"));
            background.Scale = background.Height / Game1.ScreenHeight;
            background.Position = new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);

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

            Button quitGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(300, 300), //or preferably center
                Text = "Quit Game",
            };
            quitGameButton.Click += QuitGameButton_Click;

            components = new List<IComponent>()
            {
                background,
                newGameButton,
                loadGameButton,
                quitGameButton,
            };
        }
        private void NewGameButton_Click(object sender, EventArgs e)
        {
            game.ChangeState(new TestState(game, graphicsDevice, content, input));
        }

        private void LoadGameButton_Click(object sender, EventArgs e)
        {
            //load game state from earlier
            throw new NotImplementedException();
        }

        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            game.Exit();
        }
        
    }
}
