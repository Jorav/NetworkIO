using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.menu.controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.menu.states
{
    public class MenuState : State
    {
        private List<Component> components;

        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            Texture2D buttonTexture = content.Load<Texture2D>("controls/Button");
            SpriteFont buttonFont = content.Load<SpriteFont>("fonts/Font");

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

            components = new List<Component>()
            {
                newGameButton,
                loadGameButton,
                quitGameButton,
            };
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            game.ChangeState(new GameState(game, graphicsDevice, content));
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

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            foreach (Component component in components)
                component.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
        
        public override void Update(GameTime gameTime)
        {
            foreach (Component component in components)
                component.Update(gameTime);
        }

        public override void PostUpdate()
        {
            //Remove sprites if they're not needed
            //throw new NotImplementedException();
        }

        
    }
}
