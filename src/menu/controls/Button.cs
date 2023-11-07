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
        protected internal bool isHovering;
        protected MouseState previousMouse;
        protected Sprite sprite;
        #endregion

        #region Properties
        public event EventHandler Click;
        public bool Clicked { get; private set; }
        public Color PenColour;
        protected Vector2 position;
        public virtual Vector2 Position { get { return position; } set { sprite.Position = value; position = value; } }
        protected float scale;
        public virtual float Scale { get { return scale; } set { sprite.Scale = value; scale = value; } } //doesnt work with text
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
            //Scale = 1;
        }
        public bool MouseIntersects()
        {
            Rectangle mouseRectangle = new Rectangle(currentMouse.X, currentMouse.Y, 1, 1);
            return mouseRectangle.Intersects(Rectangle);
        }
        public virtual void Update(GameTime gameTime)
        {
            previousMouse = currentMouse;
            currentMouse = Mouse.GetState();
            isHovering = false;
            HandleMouse();
        }

        protected virtual void HandleMouse()
        {
            if (MouseIntersects())
            {
                isHovering = true;
                //if (currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed)
                if (currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed)
                {
                    InvokeEvent(new EventArgs());
                }
            }
        }
        protected void InvokeEvent(EventArgs e)
        {
            Click?.Invoke(this, e);
        }

        public virtual void Draw(SpriteBatch spritebatch)
        {
            sprite.Color = Color.White;
            if (isHovering)
                sprite.Color = Color.Gray;
            //spritebatch.Draw(texture, Rectangle, color)
            sprite.Draw(spritebatch);

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
