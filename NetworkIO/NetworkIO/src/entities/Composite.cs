using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.entities
{
    class Composite : Entity
    {
        //private List<Entity> entities;
        Link[] parts;

        public Composite(Sprite sprite, Vector2 position, float rotation, float mass, float thrust, float health, bool isVisible = true, bool isCollidable = true, float friction = 0.1f, float attractionForce = 1f, float repulsionForce = 1f) : base(sprite, position, rotation, mass, thrust, health, isVisible, isCollidable, friction, attractionForce, repulsionForce)
        {
            AddLinkPositions();
        }

        public void AddEntity(Entity e, int pos) // TODO: add support for several hull parts?
        {
            if (pos >= 0 && pos < parts.Length)
                parts[pos].SetPart(e);

            /*
            if (pos >= 0 && pos < entities.Count)
            {
                if (entities[pos] != null)
                    entities[pos].Die();//dont think this is necessary
                Vector2 relativePos = Vector2.Zero;
                e.Rotation = MathHelper.ToRadians(90 * pos);
                if (pos == 0)//e.height ska subtraheras för att flyttas upp
                {
                    relativePos = Position + new Vector2(Width / 2, Height / 2) + new Vector2(Width / 2, 0) + new Vector2(e.Height, -e.Width / 2);
                }
                if (pos == 1)
                {
                    relativePos = Position + new Vector2(Width / 2, Height / 2) + new Vector2(0, -Height / 2)+ new Vector2(-e.Width / 2, -e.Height);
                }
                if (pos == 2)
                {
                    relativePos = Position + new Vector2(Width / 2, Height / 2) + new Vector2(-Width / 2, 0) + new Vector2(-e.Height, e.Width / 2);
                }
                if (pos == 3)
                {
                    relativePos = Position + new Vector2(Width / 2, Height / 2) + new Vector2(0, Height / 2) + new Vector2(e.Width / 2, e.Height);
                }
                e.Position = Position + relativePos;
                e.Origin = Position - e.Position;
                entities[pos] = e;


                
            }*/
        }

        protected void AddLinkPositions()
        {
            Vector2 relativeLinkPos;
            parts = new Link[4];

            float linkAngle = -(float)Math.PI / 2;
            Vector2 toMiddle = new Vector2(Width / 2, Height / 2);
            for(int i = 0; i < parts.Length; i++)
            {
                relativeLinkPos = toMiddle + new Vector2(toMiddle.X*(float)Math.Cos(linkAngle), toMiddle.Y * (float)Math.Sin(linkAngle)); // kanske orsakar problem men det blir en annan johans jobb
                parts[i] = new Link(relativeLinkPos, linkAngle+ (float)Math.PI / 2, this);
                linkAngle += (float)Math.PI / 2;
            }
            /*
            relativeLinkPos = new Vector2(Width / 2, Height / 2) + new Vector2(Width / 2, 0);
            parts[0] = new Link(new Vector2((float)Math.Cos(linkAngle), (float)Math.Sin(linkAngle)) * new Vector2(Width, Height) / 2, linkAngle, this);

            linkAngle += (float)Math.PI / 2;
            relativeLinkPos = new Vector2(Width / 2, Height / 2) + new Vector2(0, -Height / 2);
            parts[1] = new Link(new Vector2((float)Math.Cos(linkAngle), (float)Math.Sin(linkAngle)) * new Vector2(Width, Height) / 2, linkAngle, this);

            relativeLinkPos = new Vector2(Width / 2, Height / 2) + new Vector2(-Width / 2, 0);
            linkAngle += (float)Math.PI / 2;
            parts[2] = new Link(new Vector2((float)Math.Cos(linkAngle), (float)Math.Sin(linkAngle)) * new Vector2(Width, Height) / 2, linkAngle, this);

            relativeLinkPos = new Vector2(Width / 2, Height / 2) + new Vector2(0, Height / 2);
            linkAngle += (float)Math.PI / 2;
            parts[3] = new Link(new Vector2((float)Math.Cos(linkAngle), (float)Math.Sin(linkAngle)) * new Vector2(Width, Height) / 2, linkAngle, this);*/
        }

        public override void Move(GameTime gameTime)
        {
            base.Move(gameTime);
            for(int i = 0; i<parts.Length; i++)
            {
                if(parts[i].ConnectedEntity!=null)
                    parts[i].UpdatePart();
            }
        }
        public override void Draw(SpriteBatch sb, Matrix parentMatrix)
        {
            foreach (Link l in parts)
                if (l.ConnectedEntity != null)
                    l.ConnectedEntity.Draw(sb, parentMatrix);
            base.Draw(sb, parentMatrix);
        }

        protected class Link
        {
            public Vector2 RelativeLinkPos { get; set; } //Composite.Position+RelativePos=linkPos
            public float RelativeRotation { get; set; } //Angle connectedEntity should be rotated relative composite
            //public float InnerRotation { get; set; } //if you want an entity to rotate in its link, use e.height/2 otherwise
            private Vector2 relativeOrigin;
            private Vector2 relativePos;
            private Vector2 posChange; //probably unnecessary
            public Entity ConnectedEntity { get; set; } = null;
            public Composite Composite { get; set; }

            public Link(Vector2 relativeLinkPos, float relativeAngle, Composite composite, float innerRotation = +(float)Math.PI/2)
            {
                RelativeLinkPos = relativeLinkPos;
                RelativeRotation = relativeAngle;
                ConnectedEntity = null;
                Composite = composite;
                //InnerRotation = innerRotation;
            }

            public void SetPart(Entity e)
            {
                ConnectedEntity = e;
                relativeOrigin = Vector2.Transform(e.Origin, Matrix.CreateRotationZ(RelativeRotation));
                //UpdateRelativePos();
            }

            public void UpdatePart()
            {
                //UpdateRelativePos();
                Vector2 positionToMiddle = ConnectedEntity.Origin;
                float x = ConnectedEntity.Origin.X * (float)Math.Cos(RelativeRotation- (float)Math.PI / 2);
                float y = ConnectedEntity.Origin.Y * (float)Math.Sin(RelativeRotation- (float)Math.PI / 2);
                Vector2 linkToMiddle = /*Vector2.Transform(*/new Vector2(x, y);//, Matrix.CreateRotationZ(RelativeRotation)); //this might be magic
                relativePos = RelativeLinkPos - positionToMiddle - linkToMiddle;

                // linkToCenter = Vector2.Transform((new Vector2(p.BoundBox.Width, p.BoundBox.Height) / 2), Matrix.CreateRotationZ(RelativeAngle));
                Matrix rot = Matrix.CreateRotationZ(Composite.Rotation);
                ConnectedEntity.Position = Composite.Position + Vector2.Transform(RelativeLinkPos + linkToMiddle - Vector2.Transform(positionToMiddle, rot), rot);
                ConnectedEntity.Rotation = Composite.Rotation + RelativeRotation;

                /*
                Matrix rot = Matrix.CreateRotationZ(Composite.Rotation);
                //Vector2 pos = RelativeLinkPos + relativePos;
                Vector2 newPos= Vector2.Transform(relativePos, rot); ;//måste också använda
                ConnectedEntity.Position = Composite.Position + newPos;//IDK this needs change löl
                ConnectedEntity.Rotation = Composite.Rotation + RelativeRotation;*/

                //if (!(ConnectedEntity is Composite))
                //   ConnectedEntity.Rotation += RelativeRotation;

                /*
                 public void UpdatePart()
                {
                    UpdateRelativePos();
                    Matrix rot = Matrix.CreateRotationZ(Composite.Rotation);
                    //Vector2 pos = RelativeLinkPos + relativePos;
                    Vector2 newPos= Vector2.Transform(relativePos, rot); ;//måste också använda
                    ConnectedEntity.Position = Composite.Position + newPos;//IDK this needs change löl
                    ConnectedEntity.Rotation = Composite.Rotation + RelativeRotation;

                    //if (!(ConnectedEntity is Composite))
                    //   ConnectedEntity.Rotation += RelativeRotation;
                }
                private void UpdateRelativePos()
                {
                    Vector2 positionToMiddle = ConnectedEntity.Origin;
                    float x = ConnectedEntity.Origin.X * (float)Math.Cos(RelativeRotation-(float)Math.PI/2);
                    float y = -ConnectedEntity.Origin.Y * (float)Math.Sin(RelativeRotation - (float)Math.PI / 2);
                    Vector2 linkToMiddle = /*Vector2.Transform(
                new Vector2(x, y);//, Matrix.CreateRotationZ(RelativeRotation)); //this might be magic
                relativePos = RelativeLinkPos - positionToMiddle - linkToMiddle;
                }*/
            }
        }
    }
}
