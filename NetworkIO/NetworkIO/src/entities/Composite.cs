using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.entities
{
    class Composite
    {
        private List<Entity> entities;
        /*
        public void AddEntity()
        {
            part.LinkPosition = pos; //!!! change
            if (pos >= 0 && pos < parts.Length)
            {
                part.Carrier = this;
                //
                //
                if (TakenPositions[pos] == true)
                {

                    Parts.Remove(parts[pos].Part);//removes old part from parts
                }

                TakenPositions[pos] = true;
                if (part is CompositePart)
                {
                    ((CompositePart)part).TakenPositions[(pos + 2) % 4] = true; // reserves the new part's hull position
                }
                //
                //
                parts[pos].SetPart(part, this);
                return true;
            }
            return false;
        }

        protected abstract void AddLinkPositions();
        
        protected class Link
        {

        }
        /*
        protected class Link
        {
            public Vector2 RelativePos { get; set; }
            public float RelativeAngle { get; set; }
            public Part Part { get; set; } = null;
            private Vector2 linkToCenter;
            private Vector2 posChange;

            public Link(Vector2 relativePos, float angle)
            {
                RelativePos = relativePos;
                RelativeAngle = angle;
            }

            public void SetPart(Part p, CompositePart hull)
            {
                Part = p;
                p.Sprite.IsEvil = hull.IsEvil;
                hull.Sprite.IsEvil = hull.IsEvil;
                if (RelativeAngle == (float)Math.PI / 2 || RelativeAngle == 0) //! only works for recthull
                    linkToCenter = Vector2.Transform((new Vector2(p.BoundBox.Width, p.BoundBox.Height) / 2), Matrix.CreateRotationZ(RelativeAngle));
                else
                    linkToCenter = Vector2.Transform((new Vector2(p.BoundBox.Width, p.BoundBox.Height) / 2), Matrix.CreateRotationZ(RelativeAngle + (float)Math.PI));
                posChange = new Vector2(RelativePos.X, RelativePos.Y);
                posChange.Normalize();
                UpdatePart(hull);
            }

            public void UpdatePart(CompositePart hull)
            {
                Matrix rot = Matrix.CreateRotationZ(hull.Angle);
                Part.Position = hull.Position + Vector2.Transform(RelativePos + posChange * linkToCenter, rot);
                Part.Angle = hull.angle;
                if (!(Part is RectangularHull))
                    Part.Angle += RelativeAngle;
            }
        }
        */
    }
}
