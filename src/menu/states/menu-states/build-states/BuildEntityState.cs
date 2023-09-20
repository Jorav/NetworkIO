using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.controllers;
using NetworkIO.src.factories;
using NetworkIO.src.menu.controls;
using NetworkIO.src.movable;
using NetworkIO.src.utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.menu.states.menu_states
{
    public class BuildEntityState : BuildState
    {
        BuildOverviewState previousState;
        IControllable entityEdited;
        IDs idToBeAddded;

        public BuildEntityState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GameState gameState, Input input, BuildOverviewState previousState, Controller controllerEdited) : base(game, graphicsDevice, content, gameState, input, controllerEdited)
        {
            this.previousState = previousState;
            components = new List<IComponent>();
            menuController.AddOpenLinks();
            Sprite background = new Sprite(content.Load<Texture2D>("background/backgroundWhite"));
            background.Scale = background.Height / Game1.ScreenHeight;
            background.Position = new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
            this.entityEdited = controllerEdited.controllables[0];
            idToBeAddded = IDs.COMPOSITE;
            EntityButton addHullButton = new EntityButton(new Sprite(EntityFactory.hull), new Sprite(EntityFactory.entityButton), true)
            {
                Scale = 3,
                Position = new Vector2(Game1.ScreenWidth - EntityFactory.hull.Width - 100, 20 /*Game1.ScreenHeight - EntityFactory.hull.Height - 150*/),
            };
            addHullButton.Click += AddHullButton_Click;
            EntityButton addShooterButton = new EntityButton(new Sprite(EntityFactory.gun), new Sprite(EntityFactory.entityButton))
            {
                Scale = 3,
                Position = new Vector2(Game1.ScreenWidth - EntityFactory.hull.Width - 100, addHullButton.Position.Y+addHullButton.Rectangle.Height),
                
            };
            addShooterButton.Click += AddShooterButton_Click;
            EntityButton addSpikeButton = new EntityButton(new Sprite(EntityFactory.spike), new Sprite(EntityFactory.entityButton))
            {Scale = 3,
                Position = new Vector2(Game1.ScreenWidth - EntityFactory.hull.Width - 100, addShooterButton.Position.Y + addShooterButton.Rectangle.Height /*Game1.ScreenHeight - EntityFactory.hull.Height - 150*/),
                
            };
            addSpikeButton.Click += AddSpikeButton_Click;

            components = new List<IComponent>()
            {
                background,
                addHullButton,
                addShooterButton,
                addSpikeButton,
            };
        }

        private void AddSpikeButton_Click(object sender, EventArgs e)
        {
            idToBeAddded = IDs.SPIKE;
            menuController.requireNewClick = true;
        }

        private void AddHullButton_Click(object sender, EventArgs e)
        {
            idToBeAddded = IDs.COMPOSITE;
            menuController.requireNewClick = true;
        }
        private void AddShooterButton_Click(object sender, EventArgs e)
        {
            idToBeAddded = IDs.SHOOTER;
            menuController.requireNewClick = true;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (menuController.addEntity)
            {
                IControllable clickedC = menuController.controllableClicked;
                if (clickedC is WorldEntity clickedE && clickedE.IsFiller)
                {
                    menuController.ReplaceEntity(clickedE, EntityFactory.Create(menuController.Position, idToBeAddded));
                    menuController.AddOpenLinks();
                }
                menuController.addEntity = false;
            }
            if (menuController.clickedOutside)
            {
                bool switchState = true;
                foreach (IComponent c in components)
                    if (c is Button b && b.MouseIntersects())
                        switchState = false;
                if (switchState)
                {
                    menuController.ClearOpenLinks();
                    previousState.menuController.controllables.Remove(entityEdited);
                    previousState.menuController.AddControllable(menuController.controllables[0]);
                    previousState.menuController.MoveTo(previousState.menuController.Position);
                    previousState.menuController.Camera.InBuildScreen = true;
                    menuController.Reset();
                    game.ChangeState(previousState);
                }
                menuController.clickedOutside = false;
            }
            if (input.BuildClicked)
            {
                previousState.menuController.controllables.Remove(entityEdited);
                previousState.menuController.AddControllable(menuController.controllables[0]);
                menuController.Reset();
                gameState.Player.SetControllables(previousState.menuController.controllables); //OBS this needs edit in the future to handle stacked controllers
                gameState.Player.MoveTo(gameState.Player.Position);
                game.ChangeState(gameState);
                gameState.Player.Camera.InBuildScreen = false;
                gameState.Player.actionsLocked = false;
            }
        }
    }
}
