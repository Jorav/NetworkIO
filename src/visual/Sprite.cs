using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.menu;
using System;

namespace NetworkIO.src
{
    public class Sprite : IComponent
    {
        public Texture2D texture;
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public int Height { get { return (int)Math.Round(texture.Height * Scale); } }
        public int Width { get { return (int)Math.Round(texture.Width * Scale); } }
        public Color Color { get; set; }
        public bool isVisible = true;
        /*public Matrix LocalTransform
        {
            get
            {
                return Matrix.CreateTranslation(-Origin.X, -Origin.Y, 0f) * //origin first or last?
                       Matrix.CreateScale(Scale, Scale, 1f) *
                       Matrix.CreateRotationZ(Rotation) *
                       Matrix.CreateTranslation(Position.X, Position.Y, 0f);
            }
        }*/

        public Sprite(Texture2D texture, float scale = 1f)
        {
            this.texture = texture;
            Scale = scale;
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Color = Color.White;
        }

        public void Update(GameTime gameTime) { }

        public void Draw(SpriteBatch sb)
        {
            //Matrix globalTransform = LocalTransform * parentTransform;
            if(isVisible)
                sb.Draw(texture, Position, null, Color, Rotation, Origin, Scale, SpriteEffects.None,0f);
        }
        /*
        public void Draw(SpriteBatch sb, Color color)
        {
            //Matrix globalTransform = LocalTransform * parentTransform;
            if (isVisible)
                sb.Draw(texture, Position, null, color, Rotation, Origin, Scale, SpriteEffects.None, 0f);
        }*/

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        internal void DamageEffect()
        {
            //throw new NotImplementedException();
        }
    }
}
