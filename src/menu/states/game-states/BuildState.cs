using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace NetworkIO.src.menu.states
{
    public class BuildState : MenuState
    {
        GameState gameState;
        public BuildState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GameState gameState) : base(game, graphicsDevice, content)
        {
            this.gameState = gameState;
            Sprite background = new Sprite(content.Load<Texture2D>("background/backgroundGray"));
            background.Scale = background.Height / Game1.ScreenHeight;
            background.Position = new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
            components = new List<Component>()
            {
                background,
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (gameState.Player.BuildClicked)
                game.ChangeState(gameState);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            gameState.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);

        }
    }
}