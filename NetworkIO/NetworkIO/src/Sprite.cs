using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace NetworkIO.src
{
    public class Sprite
    {
        private Texture2D texture;
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public int Height { get { return (int)Math.Round(texture.Height * Scale); } }
        public int Width { get { return (int)Math.Round(texture.Width * Scale); } }
        public Matrix LocalTransform
        {
            get
            {
                return Matrix.CreateTranslation(-Origin.X, -Origin.Y, 0f) * //origin first or last?
                       Matrix.CreateScale(Scale, Scale, 1f) *
                       Matrix.CreateRotationZ(Rotation) *
                       Matrix.CreateTranslation(Position.X, Position.Y, 0f);
            }
        }

        public Sprite(Texture2D texture, float scale = 1f)
        {
            this.texture = texture;
            Scale = scale;
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public void Draw(SpriteBatch sb, Matrix parentTransform)
        {
            Matrix globalTransform = LocalTransform * parentTransform;
            sb.Draw(texture, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None,0f);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
