using Microsoft.Xna.Framework;
using NetworkIO.src.menu;

namespace NetworkIO.src
{
    public interface ICollidable : IIntersectable, IComponent //should be renamed
    {
        public void Collide(ICollidable c);
        public void RotateTo(Vector2 position);
        public void Accelerate(Vector2 directionalVector, float thrust);
        public object Clone();
    }
}
