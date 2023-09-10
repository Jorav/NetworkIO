using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.controllers;
using NetworkIO.src.entities.hulls;
using NetworkIO.src.factories;
using NetworkIO.src.menu.controls;
using NetworkIO.src.menu.states.menu_states;
using NetworkIO.src.movable;
using NetworkIO.src.utility;
using System;
using System.Collections.Generic;

namespace NetworkIO.src.menu.states
{
    public class BuildOverviewState : BuildState
    {
        public BuildOverviewState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GameState gameState, Input input, Controller controllerEdited) : base(game, graphicsDevice, content, gameState, input, controllerEdited)
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

            components = new List<IComponent>()
            {                
                background,
                //controller,
                addEntityButton,
                resetEntityButton,
            };
        }

        private void AddEntityButton_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            menuController.AddControllable(new EntityController(menuController.Position+new Vector2(1+(float)r.NextDouble(), (float)r.NextDouble())));
        }
        private void ResetEntityButton_Click(object sender, EventArgs e)
        {
            menuController.SetControllables(CopyEntitiesFromController(controllerEdited));
            menuController.Camera.InBuildScreen = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (input.LeftMBClicked) { //OBS will have to be adapted after controllers of controllers
                IControllable clickedE = menuController.ControllableClicked();
                if (clickedE != null)
                {
                    if (clickedE is Controller c)
                        menuController.FocusOn(clickedE);
                    else if (clickedE is EntityController ec)
                        game.ChangeState(new BuildEntityState(game, graphicsDevice, content, gameState, input, this, new Controller(new List<IControllable>() { clickedE }))); //obs, save build states?
                    //game.ChangeState(new BuildEntityState(game, graphicsDevice, content, gameState, input, this, new Controller(new List<IControllable>(ec.entities)))); //obs, save build states?
                }
            }
            //playerCopy.Update(gameTime);
            //if(gameState.Player.)
            if (input.BuildClicked)
            {
                gameState.Player.SetControllables(menuController.controllables); // OBS this needs edit in the future to handle stacked controllers
                gameState.Player.MoveTo(gameState.Player.Position);
                game.ChangeState(gameState);
                gameState.Player.actionsLocked = false;
                gameState.Player.Camera.InBuildScreen = false;
            }
        }
    }
}