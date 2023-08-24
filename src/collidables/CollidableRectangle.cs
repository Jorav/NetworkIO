using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkIO.src.collidables
{
    //OBS: Old heritage, god knows how this works
    class CollidableRectangle : ICollidable
    {
        private Vector2 UL { get; set; }
        private Vector2 DL { get; set; }
        private Vector2 DR { get; set; }
        private Vector2 UR { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Vector2 AbsolutePosition { get { return (UL + DR) / 2; } }
        private Vector2 origin;
        public Vector2 Origin
        {
            set
            {
                origin = value;
                UpdateRotation();
            }
            get
            {
                return origin;
            }
        }
        private Vector2 position;
        public Vector2 Position
        {
            set
            {
                Vector2 change = value - position;
                UL += change;
                DL += change;
                DR += change;
                UR += change;
                position = value;
            }
            get
            {
                return position;
            }
        }
        private float rotation = 0;
        public float Rotation
        {
            set
            {
                rotation = value;
                UpdateRotation();
            }
            get { return rotation; }
        }
        public CollidableRectangle(Vector2 position, float rotation, int width, int height)
        {
            UL = new Vector2(position.X, position.Y);
            DL = new Vector2(position.X, position.Y+height);
            DR = new Vector2(position.X+width, position.Y+height);
            UR = new Vector2(position.X + width, position.Y);
            this.position = position;
            Width = width;
            Height = height;
            origin = new Vector2(Width / 2, Height / 2);
            Rotation = rotation;
        }
        public bool CollidesWith(ICollidable c)
        {
            if (c is CollidableRectangle cR)
                return CollidesWithRectangle((CollidableRectangle) cR);
            //TODO: Implement collisioncheck for circle
            throw new NotImplementedException();
        }

        private void UpdateRotation()
        {
            Matrix rotationMatrix = Matrix.CreateRotationZ(rotation);
            Vector2 height = new Vector2(0, Height);
            Vector2 width = new Vector2(Width, 0);
            UL = Position + Vector2.Transform(-Origin, rotationMatrix);
            DL = Position + Vector2.Transform(-Origin + height, rotationMatrix);
            DR = Position + Vector2.Transform(-Origin + height + width, rotationMatrix);
            UR = Position + Vector2.Transform(-Origin + width, rotationMatrix);
        }

        public bool Contains(Vector2 position)
        {
            List<float> xVals = new List<float>(new float[] { UL.X, DL.X, DR.X, UR.X });
            List<float> yVals = new List<float>(new float[] { UL.Y, DL.Y, DR.Y, UR.Y });
            return xVals.Max() > position.X && xVals.Min() < position.X && yVals.Max() > position.Y && yVals.Min() < position.Y;
        }

        public bool ContainsInSpace(Vector2 positionInM, Matrix m)
        {
            Vector2 UL = Vector2.Transform(this.UL, m);
            Vector2 DL = Vector2.Transform(this.DL, m);
            Vector2 DR = Vector2.Transform(this.DR, m);
            Vector2 UR = Vector2.Transform(this.UR, m);
            List<float> xVals = new List<float>(new float[] { UL.X, DL.X, DR.X, UR.X });
            List<float> yVals = new List<float>(new float[] { UL.Y, DL.Y, DR.Y, UR.Y });
            return xVals.Max() > positionInM.X && xVals.Min() < positionInM.X && yVals.Max() > positionInM.Y && yVals.Min() < positionInM.Y;
        }

        public bool CollidesWithRectangle(CollidableRectangle r)
        {
            Vector2[] axes = GenerateAxes(r);
            float[] scalarA = new float[4];
            float[] scalarB = new float[4];
            foreach (Vector2 axis in axes)
            {
                scalarA[0] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(UL, axis) / axis.LengthSquared()));
                scalarA[1] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(DL, axis) / axis.LengthSquared()));
                scalarA[2] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(DR, axis) / axis.LengthSquared()));
                scalarA[3] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(UR, axis) / axis.LengthSquared()));
                scalarB[0] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(r.UL, axis) / axis.LengthSquared()));
                scalarB[1] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(r.DL, axis) / axis.LengthSquared()));
                scalarB[2] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(r.DR, axis) / axis.LengthSquared()));
                scalarB[3] = Vector2.Dot(axis, Vector2.Multiply(axis, Vector2.Dot(r.UR, axis) / axis.LengthSquared()));
                if (scalarB.Max() < scalarA.Min() || scalarA.Max() < scalarB.Min())
                    return false;
            }
            return true;
        }
        private Vector2[] GenerateAxes(CollidableRectangle r)
        {
            Vector2[] axes = new Vector2[4];
            axes[0] = new Vector2(UR.X - UL.X, UR.Y - UL.Y);
            axes[1] = new Vector2(UR.X - DR.X, UR.Y - DR.Y);
            axes[2] = new Vector2(r.UL.X - r.DL.X, r.UL.Y - r.DL.Y);
            axes[3] = new Vector2(r.UL.X - r.UR.X, r.UL.Y - r.UR.Y);
            return axes;
        }
    }
}