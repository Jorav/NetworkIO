using Microsoft.Xna.Framework;

namespace NetworkIO.src
{
    public interface ICollidable : IIntersectable
    {
        public void Collide(ICollidable c);
        public void Update(GameTime gameTime);
    }
}
