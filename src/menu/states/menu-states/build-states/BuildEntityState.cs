using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.controllers;
using NetworkIO.src.factories;
using NetworkIO.src.movable;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.menu.states.menu_states
{
    public class BuildEntityState : BuildState
    {
        BuildOverviewState previousState;
        IControllable entityEdited;
        public BuildEntityState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GameState gameState, Input input, BuildOverviewState previousState, Controller controllerEdited) : base(game, graphicsDevice, content, gameState, input, controllerEdited)
        {
            this.previousState = previousState;
            components = new List<IComponent>();
            menuController.AddOpenLinks();
            Sprite background = new Sprite(content.Load<Texture2D>("background/backgroundWhite"));
            background.Scale = background.Height / Game1.ScreenHeight;
            background.Position = new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
            this.entityEdited = controllerEdited.controllables[0];

            components = new List<IComponent>()
            {
                background,
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (input.leftMBClicked)
            {
                IControllable clicked = menuController.EntityClicked();
                if(clicked == null) {
                    previousState.menuController.controllables.Remove(entityEdited);
                    previousState.menuController.AddControllable(menuController.controllables[0]);
                    previousState.menuController.MoveTo(previousState.menuController.Position);
                    menuController.Reset();
                    game.ChangeState(previousState);
                }
                else
                {
                    if (clicked is WorldEntity clickedE && clickedE.IsFiller)
                    {
                        menuController.ReplaceEntity(clickedE, EntityFactory.Create(menuController.Position, utility.IDs.SHOOTER));
                        menuController.AddOpenLinks();
                    }
                        
                }
            }
            if (input.BuildClicked)
            {
                previousState.menuController.controllables.Remove(entityEdited);
                previousState.menuController.AddControllable(menuController.controllables[0]);
                menuController.Reset();
                gameState.Player.SetControllables(previousState.menuController.controllables); //OBS this needs edit in the future to handle stacked controllers
                gameState.Player.MoveTo(gameState.Player.Position);
                game.ChangeState(gameState);
                gameState.Player.actionsLocked = false;
            }
        }
    }
}
