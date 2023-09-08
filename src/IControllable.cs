using Microsoft.Xna.Framework;
using NetworkIO.src.menu;

namespace NetworkIO.src
{
    public interface IControllable : IIntersectable, IComponent //should be renamed
    {
        public void Collide(IControllable c);
        public void RotateTo(Vector2 position);
        public void Accelerate(Vector2 directionalVector, float thrust);
        public void Shoot(GameTime gameTime);
        public object Clone();
        public bool ContainsInSpace(Vector2 position, Matrix transform);
    }
}
