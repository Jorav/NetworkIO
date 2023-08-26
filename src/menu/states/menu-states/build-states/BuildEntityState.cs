using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.menu.states.menu_states
{
    public class BuildEntityState : BuildState
    {
        BuildOverviewState previousState;
        public BuildEntityState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GameState gameState, Input input, BuildOverviewState previousState, Controller controller) : base(game, graphicsDevice, content, gameState, input, controller)
        {
            this.previousState = previousState;
            components = new List<Component>();

            Sprite background = new Sprite(content.Load<Texture2D>("background/backgroundWhite"));
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
            if (input.leftMBClicked)
            {
                Entity clickedE = menuController.MouseOnEntity();
                if(clickedE == null)
                    game.ChangeState(previousState);
            }
            if (input.BuildClicked)
            {
                gameState.Player.SetEntities(previousState.menuController.entities);
                gameState.Player.MoveTo(gameState.Player.Position);
                game.ChangeState(gameState);
                gameState.Player.actionsLocked = false;
            }
        }
    }
}
