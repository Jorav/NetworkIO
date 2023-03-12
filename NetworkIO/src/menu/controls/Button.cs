using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.menu.controls
{
    public class Button : Component
    {
        #region Fields
        private MouseState currentMouse;
        private SpriteFont font;
        private bool isHovering;
        private MouseState previousMouse;
        private Texture2D texture;
        #endregion

        #region Properties
        public event EventHandler Click;
        public bool Clicked { get; private set; }
        public Color PenColour;
        public Vector2 Position;
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
            }
        }
        public String Text { get; set; }
        #endregion

        #region Methods
        public Button(Texture2D texture, SpriteFont font) 
        {
            this.texture = texture;
            this.font = font;
            PenColour = Color.Black;
        }
        
        public void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            Color color = Color.White;
            if (isHovering)
                color = Color.Gray;
            spritebatch.Draw(texture, Rectangle, color);

            if (!string.IsNullOrEmpty(Text))
            {
                float x = (Rectangle.X + (Rectangle.Width / 2)) - font.MeasureString(Text).X / 2;
                float y = (Rectangle.Y + (Rectangle.Height / 2)) - font.MeasureString(Text).Y / 2;
                spritebatch.DrawString(font, Text, new Vector2(x, y), PenColour);
            }

        }
        
        public void Update(GameTime gameTime)
        {
            previousMouse = currentMouse;
            currentMouse = Mouse.GetState();
            Rectangle mouseRectangle = new Rectangle(currentMouse.X, currentMouse.Y, 1, 1);
            isHovering = false;
            if (mouseRectangle.Intersects(Rectangle)) { 
                isHovering = true;
                if (currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }
        #endregion
    }
}
