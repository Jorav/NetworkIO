using Microsoft.Xna.Framework;
using NetworkIO.src.collidables;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.collission_detectors
{
    public interface CollisionDetector : IIntersectable
    {
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public bool Contains(Vector2 position);
        public bool ContainsInSpace(Vector2 positionInM, Matrix m);
        public bool CollidesWithCircle(CollidableCircle cc);
        public bool CollidesWithRectangle(CollidableRectangle r);
        public void StretchTo(CollisionDetector cd);
        public void StopStretch();
        public bool CollidesWithStretch(CollisionDetector cd);
    }
}
