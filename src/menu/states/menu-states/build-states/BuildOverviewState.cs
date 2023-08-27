using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.controllers;
using NetworkIO.src.factories;
using NetworkIO.src.menu.controls;
using NetworkIO.src.menu.states.menu_states;
using NetworkIO.src.utility;
using System;
using System.Collections.Generic;

namespace NetworkIO.src.menu.states
{
    public class BuildOverviewState : BuildState
    {
        public BuildOverviewState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GameState gameState, Input input, Controller controller) : base(game, graphicsDevice, content, gameState, input, controller)
        {
            Sprite background = new Sprite(content.Load<Texture2D>("background/backgroundWhite"));
            background.Scale = background.Height / Game1.ScreenHeight;
            background.Position = new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
            Texture2D buttonTexture = content.Load<Texture2D>("controls/Button");
            SpriteFont buttonFont = content.Load<SpriteFont>("fonts/Font");
            Button addEntityButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(Game1.ScreenWidth-buttonTexture.Width-100, Game1.ScreenHeight - buttonTexture.Height-150), //make this vary with Zoom
                Text = "Add Hull",
            };
            Button resetEntityButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(Game1.ScreenWidth - buttonTexture.Width - 100, Game1.ScreenHeight - buttonTexture.Height - 100), //make this vary with Zoom
                Text = "Reset",
            };
            addEntityButton.Click += AddEntityButton_Click;
            resetEntityButton.Click += ResetEntityButton_Click;

            components = new List<Component>()
            {                
                background,
                //controller,
                addEntityButton,
                resetEntityButton,
            };
        }

        private void AddEntityButton_Click(object sender, EventArgs e)
        {
            menuController.entities.Add(EntityFactory.Create(menuController.Position, IDs.SHOOTER));
        }
        private void ResetEntityButton_Click(object sender, EventArgs e)
        {
            menuController.SetEntities(CopyEntitiesFromController(controllerEdited));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (input.leftMBClicked) { 
                Entity clickedE = menuController.MouseOnEntity();
                if (clickedE != null)
                {
                    game.ChangeState(new BuildEntityState(game, graphicsDevice, content, gameState, input, this, new EntityController(new List<Entity>() { clickedE }))); //obs, save build states?
                }
            }
            //playerCopy.Update(gameTime);
            //if(gameState.Player.)
            if (input.BuildClicked)
            {
                gameState.Player.SetEntities(menuController.entities);
                gameState.Player.MoveTo(gameState.Player.Position);
                game.ChangeState(gameState);
                gameState.Player.actionsLocked = false;
            }
        }
    }
}