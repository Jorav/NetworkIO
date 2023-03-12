using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NetworkIO.src.menu.states
{
    public class BuilderState : MenuState
    {
        GameState gameState;
        public BuilderState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GameState gameState) : base(game, graphicsDevice, content)
        {
            this.gameState = gameState;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (gameState.Player.BuildClicked)
                game.ChangeState(gameState);
        }
    }
}