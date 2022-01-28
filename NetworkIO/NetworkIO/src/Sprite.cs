using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace NetworkIO.src
{
    class Sprite
    {
        private Texture2D texture;
        public float Scale { set; get; }
        public float Rotation;
        public Vector2 Position;
        public Vector2 Origin;
        public int Height { get { return (int)Math.Round(texture.Height * Scale); } }
        public int Width { get { return (int)Math.Round(texture.Width * Scale); } }

        public Sprite(Texture2D texture, float scale = 1)
        {
            this.texture = texture;
            Scale = scale;
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, Position, null, Color.White, Rotation + MathHelper.ToRadians(90), Origin, Scale, SpriteEffects.None,0f);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
