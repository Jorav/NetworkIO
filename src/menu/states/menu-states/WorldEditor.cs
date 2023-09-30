using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NetworkIO.src.controllers;
using NetworkIO.src.menu.controls;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NetworkIO.src.menu.states.menu_states
{
    public class WorldEditor : MenuState
    {
        public Camera Camera { get; set; }
        List<Background> backgrounds;
        List<IControllable> controllers;
        bool previousLeftMBDown;
        bool dragging = true;
        Vector2 draggingRelativePosition;
        Input input;
        IControllable clicked;

        public WorldEditor(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, Input input, [OptionalAttribute] GameState gameState) : base(game, graphicsDevice, content, input)
        {
            Camera = new Camera();
            this.input = input;
            input.Camera = Camera;
            backgrounds = new List<Background>();
            controllers = new List<IControllable>();
            Texture2D buttonTexture = content.Load<Texture2D>("controls/Button");
            SpriteFont buttonFont = content.Load<SpriteFont>("fonts/Font");
            Button addControllerButton = new Button(new Sprite(buttonTexture), buttonFont)
            {
                Position = new Vector2(Game1.ScreenWidth - buttonTexture.Width - 100, Game1.ScreenHeight - buttonTexture.Height - 150), //make this vary with Zoom
                Text = "Add Controller",
            };
            addControllerButton.Click += AddControllerButton_Click;
            components = new List<IComponent>()
            {
                addControllerButton,
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Camera.Update();
            Vector2 mousePosition = Mouse.GetState().Position.ToVector2();



            if (input.LeftMBDown && !previousLeftMBDown)
            {
                foreach (IControllable c in controllers)
                {
                    IControllable clicked = c.ControllableContainingInSpace(mousePosition, Camera.Transform);
                    if (clicked != null)
                    {
                        this.clicked = c;
                        dragging = true;
                        draggingRelativePosition = c.Position - input.MousePositionGameCoords;
                    }
                }
                if (!dragging)
                    this.clicked = null;
            }

            if (clicked != null && dragging && input.LeftMBDown)
                clicked.Position = input.MousePositionGameCoords+draggingRelativePosition;
            if (!input.LeftMBDown && previousLeftMBDown)
                dragging = false;
            //if (previousLeftMBDown && !input.LeftMBDown && clicked != null && mousePosition != mousePositionClicked)
                //clicked = null;        
            previousLeftMBDown = input.LeftMBDown;

            if (input.BuildClicked && clicked != null && clicked is Controller controller)
                game.ChangeState(new BuildOverviewState(game, graphicsDevice, content, this, input, controller));
            if (input.PauseClicked)
                game.ChangeState(new PauseState(game, graphicsDevice, content, this, input));
        }

        private void AddControllerButton_Click(object sender, EventArgs e)
        {
            controllers.Add(new Controller(Camera.Position));
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
            base.Draw(gameTime, spriteBatch);
        }
    }
}
