using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.controllers;
using NetworkIO.src.menu.controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.menu.states.menu_states
{
    public class WorldEditor : MenuState
    {
        Camera c;
        public WorldEditor(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GameState gameState, Input input) : base(game, graphicsDevice, content, input)
        {

            Texture2D buttonTexture = content.Load<Texture2D>("controls/Button");
            SpriteFont buttonFont = content.Load<SpriteFont>("fonts/Font");
            Button addControllerButton = new Button(new Sprite(buttonTexture), buttonFont)
            {
                Position = new Vector2(Game1.ScreenWidth - buttonTexture.Width - 100, Game1.ScreenHeight - buttonTexture.Height - 150), //make this vary with Zoom
                Text = "Add Controller",
            };
            addControllerButton.Click += AddControllerButton_Click;
        }

        private void AddControllerButton_Click(object sender, EventArgs e)
        {
            //new EntityController()
        }
    }
}
