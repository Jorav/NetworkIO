﻿using Microsoft.Xna.Framework;
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
        public BuildOverviewState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, State previousState, Input input, Controller controllerEdited, MenuController menuController = null) : base(game, graphicsDevice, content, previousState, input, controllerEdited, menuController)
        {
            Texture2D buttonTexture = content.Load<Texture2D>("controls/Button");
            SpriteFont buttonFont = content.Load<SpriteFont>("fonts/Font");
            Button addEntityButton = new Button(new Sprite(buttonTexture), buttonFont)
            {
                Position = new Vector2(Game1.ScreenWidth - buttonTexture.Width - 100, Game1.ScreenHeight - buttonTexture.Height - 150), //make this vary with Zoom
                Text = "Add Hull",
            };
            Button resetEntityButton = new Button(new Sprite(buttonTexture), buttonFont)
            {
                Position = new Vector2(Game1.ScreenWidth - buttonTexture.Width - 100, Game1.ScreenHeight - buttonTexture.Height - 100), //make this vary with Zoom
                Text = "Reset",
            };
            addEntityButton.Click += AddEntityButton_Click;
            resetEntityButton.Click += ResetEntityButton_Click;

            components = new List<IComponent>()
            {
                //controller,
                addEntityButton,
                resetEntityButton,
            };
        }

        private void AddEntityButton_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            EntityController ec = new EntityController(menuController.Position + new Vector2((float)r.NextDouble() - 0.5f, (float)r.NextDouble() - 0.5f));
            //while (menuController.CollidesWith(ec))
            //ec.MoveTo(ec.Position + 10* new Vector2((float)r.NextDouble() - 0.5f, (float)r.NextDouble() - 0.5f));
            menuController.AddControllable(ec);
        }
        private void ResetEntityButton_Click(object sender, EventArgs e)
        {
            menuController.SetControllables(CopyEntitiesFromController(controllerEdited));
            menuController.Camera.InBuildScreen = true;
            this.menuController.Color = Color.White;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            bool interactWithMenuController = true;
            foreach (IComponent c in components)
                if (c is Button b && b.MouseIntersects())
                    interactWithMenuController = false;
            if (interactWithMenuController)
            {
                if (menuController.clickedOnControllable)
                {
                    IControllable clickedC = menuController.controllableClicked;
                    if (clickedC is Controller c)
                        ;// menuController.FocusOn(clickedC);
                    else if (clickedC is EntityController ec)
                        game.ChangeState(new BuildEntityState(game, graphicsDevice, content, previousState, input, this, new Controller(new List<IControllable>() { clickedC }), menuController)); //obs, save build states?
                    else if (clickedC is WorldEntity w)
                        game.ChangeState(new BuildEntityState(game, graphicsDevice, content, previousState, input, this, new Controller(new List<IControllable>() { w.Manager }), menuController)); //obs, save build states?
                                                                                                                                                                                         //menuController.Camera.AutoAdjustZoom = true;
                    menuController.newClickRequired = true;
                    menuController.clickedOnControllable = false;
                    menuController.FocusOn(clickedC);
                }
                else if (menuController.removeEntity)
                {
                    IControllable clickedC = menuController.controllableClicked;
                    if (clickedC is Controller c)
                        menuController.Remove(clickedC);
                    else if (clickedC is EntityController ec)
                        menuController.Remove(ec);
                    else if (clickedC is WorldEntity w)
                        menuController.Remove(w.Manager);

                    menuController.removeEntity = false;
                }
                else if (menuController.clickedOutside)
                {
                    menuController.DeFocus();
                    menuController.clickedOutside = false;
                }
            }
            else
            {
                menuController.newClickRequired = true;
                menuController.clickedOutside = false;
                menuController.removeEntity = false;
                menuController.clickedOnControllable = false;
            }
            if (input.BuildClicked)
            {
                BuildClicked();
            }
        }

        public void BuildClicked()
        {
            Vector2 position = controllerEdited.Position;
            menuController.Reset();
            controllerEdited.SetControllables(menuController.Controllables);
            controllerEdited.Position = position;
            if (previousState is IPlayable p)
            {
                p.Player.actionsLocked = false;
                p.Camera.InBuildScreen = false;
            }
            if (previousState is WorldEditor editor)
            {
                editor.Camera.InBuildScreen = false;
            }
            game.ChangeState(previousState);
            controllerEdited.Color = originalColor;
            //menuController.Camera.AutoAdjustZoom = true;
        }
    }
}