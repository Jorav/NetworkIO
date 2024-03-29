﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NetworkIO.src.controllers;
using NetworkIO.src.factories;
using NetworkIO.src.menu.controls;
using NetworkIO.src.utility;
using NetworkIO.src.visual;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NetworkIO.src.menu.states.menu_states
{
    public class WorldEditor : MenuState, IPlayable
    {
        public Camera Camera { get; set; }
        List<Background> backgrounds;
        List<IControllable> controllers;
        bool previousLeftMBDown;
        bool previousRightMBDown;
        bool dragging = true;
        Vector2 draggingRelativePosition;
        private IControllable clicked;
        private FadingText Warning;
        private float zoom;
        protected float Zoom { get { return zoom; } set { zoom = value; Camera.Zoom = value; } }
        public IControllable Clicked
        {
            set
            {
                IControllable previousClicked = clicked;
                if (previousClicked != null)
                    previousClicked.Color = Color.White;
                if(value != null)
                    value.Color = Color.Turquoise;
                clicked = value;
            }
            get
            {
                return clicked;
            }
        }
        public Player Player { get; set; }
        IDs IDSelected;

        public WorldEditor(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, Input input, [OptionalAttribute] GameState gameState) : base(game, graphicsDevice, content, input)
        {
            Player = new Player(input);
            Camera = new Camera();
            zoom = Camera.Zoom;
            input.Camera = Camera;
            this.input = input;
            backgrounds = new List<Background>();
            controllers = new List<IControllable>();
            controllers.Add(Player);
            Clicked = Player;
            Texture2D buttonTexture = content.Load<Texture2D>("controls/Button");
            SpriteFont buttonFont = content.Load<SpriteFont>("fonts/Font");
            Warning = new FadingText("Need a Player-controller to start", new Vector2(Game1.ScreenWidth/2/1.5f, Game1.ScreenHeight/2/1.5f), buttonFont, 1.5f) { originalColor = Color.Red };
            Button addControllerButton = new Button(new Sprite(buttonTexture), buttonFont)
            {
                Position = new Vector2(Game1.ScreenWidth - buttonTexture.Width - 100, Game1.ScreenHeight - buttonTexture.Height - 150), //make this vary with Zoom
                Text = "Add Controller",
            };
            addControllerButton.Click += AddControllerButton_Click;
            IDs[] ids = new IDs[]
                {
                    IDs.CONTROLLER_DEFAULT,
                    IDs.CHASER_AI,
                    IDs.PLAYER
                };
            this.IDSelected = ids[0];
            DropDownButton setControllerButton = new DropDownButton(new Sprite(buttonTexture), buttonFont, ids)
            {
                Position = new Vector2(Game1.ScreenWidth - buttonTexture.Width - 100, 20),
            };
            setControllerButton.Click += SetControllerButton_Click;

            components = new List<IComponent>()
            {
                addControllerButton,
                setControllerButton,
            };
        }

        private void SetControllerButton_Click(object sender, EventArgs e)
        {
            if(sender is DropDownButton button)
            {
                IDs id = button.currentSelection.Item2;
                if(clicked != null)
                {
                    if(clicked is Player p)
                    {
                        //p.controllables.Remove()
                    }
                }
                else
                {
                    IDSelected = id;
                }
            }
                
            //throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HandleWASD();
            HandleScroll();
            Camera.Update();

            Vector2 mousePosition = Mouse.GetState().Position.ToVector2();
            HandleLeftClick(mousePosition);
            HandleRightClick(mousePosition);


            //drag controller
            if (clicked != null && dragging && input.LeftMBDown)
                clicked.Position = input.MousePositionGameCoords + draggingRelativePosition;
            //clicked.Position = input.MousePositionGameCoords + draggingRelativePosition;
            //disable drag if clicked outside
            if (!input.LeftMBDown && previousLeftMBDown)
                dragging = false;

            previousLeftMBDown = input.LeftMBDown;
            previousRightMBDown = input.RightMBDown;

            if (input.BuildClicked)
                if (clicked != null && clicked is Controller controller)
                    game.ChangeState(new BuildOverviewState(game, graphicsDevice, content, this, input, controller));
            if (input.EnterClicked)
            {
                //bool playerExists = false;
                //foreach (IControllable c in controllers)
                //    if (c is Player)
                //       playerExists = true;
                if(Player.Controllables.Count != 0)
                {
                    Clicked = null;
                    game.ChangeState(new GameState(game, graphicsDevice, content, input, this, controllers));
                }
                else
                    Warning.Display();
            }
            if (input.PauseClicked)
                game.ChangeState(new PauseState(game, graphicsDevice, content, this, input));
            Warning.Update(gameTime);
            Zoom = zoom;
        }

        private void HandleScroll()
        {
            int currentScrollValue = input.ScrollValue;
            if (input.PreviousScrollValue - currentScrollValue != 0)
            {
                Zoom /= (float)Math.Pow(0.999, (currentScrollValue - input.PreviousScrollValue));
            }
        }

        private void HandleWASD()
        {
            float moveConstant = 5/Camera.Zoom;
            if (Keyboard.GetState().IsKeyDown(input.Up) ^ Keyboard.GetState().IsKeyDown(input.Down))
            {
                if (Keyboard.GetState().IsKeyDown(input.Up))
                {
                    Camera.Position += new Vector2(0, -moveConstant);
                }
                else if (Keyboard.GetState().IsKeyDown(input.Down))
                {
                    Camera.Position += new Vector2(0, moveConstant);
                }
            }
            if (Keyboard.GetState().IsKeyDown(input.Left) ^ Keyboard.GetState().IsKeyDown(input.Right))
            {
                if (Keyboard.GetState().IsKeyDown(input.Left))
                {
                    Camera.Position += new Vector2(-moveConstant, 0);
                }
                else if (Keyboard.GetState().IsKeyDown(input.Right))
                {
                    Camera.Position += new Vector2(moveConstant, 0);
                }
            }
        }

        private void HandleRightClick(Vector2 mousePosition)
        {
            if (input.RightMBDown && !previousRightMBDown)
            {
                List<IControllable> temp = new List<IControllable>();
                foreach (IControllable c in controllers)
                {
                    IControllable clicked = c.ControllableContainingInSpace(mousePosition, Camera.Transform);
                    if (clicked != null)
                    {
                        if (c is Player)
                            Player.Remove(clicked.Manager);
                        else
                            temp.Add(c);
                    }
                }
                foreach (IControllable c in temp)
                    controllers.Remove(c);
            }
        }

        private void HandleLeftClick(Vector2 mousePosition)
        {
            if (input.LeftMBDown && !previousLeftMBDown)
            {
                bool deselect = true;
                foreach (IControllable c in controllers)
                {
                    IControllable clicked = c.ControllableContainingInSpace(mousePosition, Camera.Transform);
                    if (clicked != null)
                    {
                        Clicked = c;
                        dragging = true;
                        //draggingRelativePosition = c.Position - input.MousePositionGameCoords;
                        draggingRelativePosition = (c.Position- input.MousePositionGameCoords);
                        deselect = false;
                    }
                }
                if(deselect)
                {
                    dragging = false;
                    Clicked = null;
                }
                if (!dragging)
                    Clicked = null;
            }
        }

        private void AddControllerButton_Click(object sender, EventArgs e)
        {
            Vector2 spawnPosition = Camera.Position;
            bool add = true;
            foreach (IControllable c in controllers)
                if (c.Position == spawnPosition)
                    add = false;
            if (add)
            {
                IControllable c;
                if (IDSelected == IDs.PLAYER)
                {
                    c = new EntityController(Camera.Position);
                    Player.AddControllable(c);
                }
                else
                {
                    c = ControllerFactory.Create(Camera.Position, IDSelected);
                    controllers.Add(c);
                }
                    
                Clicked = c;
            }
                
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: Camera.Transform);
            game.GraphicsDevice.Clear(Color.DarkGray);
            foreach (Background b in backgrounds)
            {
                b.Draw(spriteBatch);
            }

            foreach (IControllable c in controllers)
            {
                c.Draw(spriteBatch);
            }
            spriteBatch.End();
            spriteBatch.Begin(transformMatrix: Matrix.CreateScale(1.5f));
            Warning.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime, spriteBatch);
        }
    }
}
