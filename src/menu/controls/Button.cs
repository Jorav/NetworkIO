using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.menu.controls
{
    public class Button : IComponent
    {
        #region Fields
        protected MouseState currentMouse;
        protected SpriteFont font;
        protected bool isHovering;
        protected MouseState previousMouse;
        protected Sprite sprite;
        #endregion

        #region Properties
        public event EventHandler Click;
        public bool Clicked { get; private set; }
        public Color PenColour;
        private Vector2 position;
        public Vector2 Position { get { return position; } set { sprite.Position = value; position = value; } }
        private float scale;
        public float Scale { get { return scale; } set { sprite.Scale = value; scale = value; } } //doesnt work with text
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);
            }
        }
        public String Text { get; set; }
        #endregion

        #region Methods
        public Button(Sprite sprite, SpriteFont font = null) 
        {
            this.sprite = sprite;
            this.font = font;
            PenColour = Color.Black;
            sprite.Origin = Vector2.Zero;
            Scale = 1;
        }
        
        public void Update(GameTime gameTime)
        {
            previousMouse = currentMouse;
            currentMouse = Mouse.GetState();
            Rectangle mouseRectangle = new Rectangle(currentMouse.X, currentMouse.Y, 1, 1);
            isHovering = false;
            if (mouseRectangle.Intersects(Rectangle)) { 
                isHovering = true;
                //if (currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed)
                if (currentMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }
        public void Draw(SpriteBatch spritebatch)
        {
            Color color = Color.White;
            if (isHovering)
                color = Color.Gray;
            //spritebatch.Draw(texture, Rectangle, color)
            sprite.Draw(spritebatch, color);

            if (!string.IsNullOrEmpty(Text))
            {
                float x = (Rectangle.X + (Rectangle.Width / 2)) - font.MeasureString(Text).X / 2;
                float y = (Rectangle.Y + (Rectangle.Height / 2)) - font.MeasureString(Text).Y / 2;
                spritebatch.DrawString(font, Text, new Vector2(x, y), PenColour);
            }

        }
        #endregion
    }
}
